using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Genetical_Algorithm
{
    // Made in December 2019 by Piotr Ulaszewski.

    public partial class Form1 : Form
    {
        // each person has a chromosome = a 10-bit value describing which items he owns
        // weight = weight of those items, value = value of those items, chances = percent value of success in whole population of 20 people
        // chosen = how many times was he chosen in the roulette
        List<Point5D> people = new List<Point5D>();

        // 10 items (X = item nr, Y = weight of item, Z = value of item
        List<Point3D> items = new List<Point3D>();

        int iterations = 0;
        int steps = 0;
        int weightmax = 0;
        float pcrossing = 0;
        float pmutation = 0;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Draw_Click(object sender, EventArgs e)
        {
            // clear list of items (x = nr of item, y = weight of item, z = value of item
            items.Clear();

            // fill list of items (there are 10)
            int y, z;
            if ((int.TryParse(w1.Text, out y)) && (int.TryParse(v1.Text, out z))) items.Add(new Point3D(1, y, z));
            if ((int.TryParse(w2.Text, out y)) && (int.TryParse(v2.Text, out z))) items.Add(new Point3D(2, y, z));
            if ((int.TryParse(w3.Text, out y)) && (int.TryParse(v3.Text, out z))) items.Add(new Point3D(3, y, z));
            if ((int.TryParse(w4.Text, out y)) && (int.TryParse(v4.Text, out z))) items.Add(new Point3D(4, y, z));
            if ((int.TryParse(w5.Text, out y)) && (int.TryParse(v5.Text, out z))) items.Add(new Point3D(5, y, z));
            if ((int.TryParse(w6.Text, out y)) && (int.TryParse(v6.Text, out z))) items.Add(new Point3D(6, y, z));
            if ((int.TryParse(w7.Text, out y)) && (int.TryParse(v7.Text, out z))) items.Add(new Point3D(7, y, z));
            if ((int.TryParse(w8.Text, out y)) && (int.TryParse(v8.Text, out z))) items.Add(new Point3D(8, y, z));
            if ((int.TryParse(w9.Text, out y)) && (int.TryParse(v9.Text, out z))) items.Add(new Point3D(9, y, z));
            if ((int.TryParse(w10.Text, out y)) && (int.TryParse(v10.Text, out z))) items.Add(new Point3D(10, y, z));

            // we have to fill all items and weightmax, probability of crossing and probability of mutation
            if (items.Count < 10 || !int.TryParse(wmax.Text, out weightmax) || !float.TryParse(pk.Text, out pcrossing) || !float.TryParse(pm.Text, out pmutation))
            {
                MessageBox.Show("Please enter all data!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (pcrossing < 0 || pcrossing > 1)
            {
                MessageBox.Show("Crossing probability needs a value between 0 and 1!", "Data error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (pmutation < 0 || pmutation > 1)
            {
                MessageBox.Show("Mutation probability needs a value between 0 and 1!", "Data error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (weightmax <= 0)
            {
                MessageBox.Show("Maximum weight can not be 0 or less than 0", "Data error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // clear list of people
            people.Clear();
            Random rnd = new Random();

            // we have 20 persons - which we draw from a random pool at the start
            for (int i = 0; i < 20; i++)
            {
                // (chromosom) get a random number from 0 to 1023 (because we have 10 bitów), (weight) count the weight of all items, (value) count the value of all items
                int losowa = rnd.Next(1024);
                people.Add(new Point5D(losowa, CalcWeight(losowa), CalcValue(losowa), 0, 0, false, false));
            }

            // show the list of people
            Display();

            // go to first iteration
            iterations = 1;
            iteracja.Text = "Algorithm iteration number " + iterations.ToString();
            steps = 1;
        }

        private void Step_Click(object sender, EventArgs e)
        {
            if (iterations == 0 || weightmax == 0) return;

            // sum of values from all backpacks
            int sumawartości = 0;
            Random rnd = new Random();

            switch (steps)
            {
                // mutation until weight is not within range
                case 1:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Adaptation assessment - Mutation until the weight is less than " + (weightmax + 1).ToString();
                    foreach (Point5D p in people)
                    {
                        if (p.weight > weightmax)
                        {
                            // do mutations wntil weight is higher than max selected (there can be more mutations than 1)
                            while (p.weight > weightmax)
                            {
                                // we draw a random position from 0 to 9 because we have 10 bits
                                int random = rnd.Next(10);
                                int position = (int)Math.Pow(2.00, random); // 2.00 ^ random, position 1,2,4,8,16,32,64,128,256,512
                                int value = (int)p.chromosome;
                                if ((value & position) > 0) value ^= position; // we change 1 bit only from 1 to 0
                                p.chromosome = value;
                                p.weight = CalcWeight((int)p.chromosome);
                                p.value = CalcValue((int)p.chromosome);
                            }
                        }
                    }
                    // show again
                    Display();
                    break;

                // chances in percent
                case 2:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Determination of percentage chances";
                    foreach (Point5D p in people)
                    {
                        // calculate chances in percent
                        // sum of values of all backpacks
                        sumawartości = 0;
                        for (int i = 0; i < 20; i++) sumawartości += (int)people[i].value;
                        p.chances = sumawartości != 0 ? ((float)p.value / (float)sumawartości) * 100.0f : 0;
                    }

                    // show again and display chances in percent
                    Display();
                    o1.Text += "  (" + people[0].chances.ToString("0.00") + "%)";
                    o2.Text += "  (" + people[1].chances.ToString("0.00") + "%)";
                    o3.Text += "  (" + people[2].chances.ToString("0.00") + "%)";
                    o4.Text += "  (" + people[3].chances.ToString("0.00") + "%)";
                    o5.Text += "  (" + people[4].chances.ToString("0.00") + "%)";
                    o6.Text += "  (" + people[5].chances.ToString("0.00") + "%)";
                    o7.Text += "  (" + people[6].chances.ToString("0.00") + "%)";
                    o8.Text += "  (" + people[7].chances.ToString("0.00") + "%)";
                    o9.Text += "  (" + people[8].chances.ToString("0.00") + "%)";
                    o10.Text += "  (" + people[9].chances.ToString("0.00") + "%)";
                    o11.Text += "  (" + people[10].chances.ToString("0.00") + "%)";
                    o12.Text += "  (" + people[11].chances.ToString("0.00") + "%)";
                    o13.Text += "  (" + people[12].chances.ToString("0.00") + "%)";
                    o14.Text += "  (" + people[13].chances.ToString("0.00") + "%)";
                    o15.Text += "  (" + people[14].chances.ToString("0.00") + "%)";
                    o16.Text += "  (" + people[15].chances.ToString("0.00") + "%)";
                    o17.Text += "  (" + people[16].chances.ToString("0.00") + "%)";
                    o18.Text += "  (" + people[17].chances.ToString("0.00") + "%)";
                    o19.Text += "  (" + people[18].chances.ToString("0.00") + "%)";
                    o20.Text += "  (" + people[19].chances.ToString("0.00") + "%)";
                    suma.Text = "The last sum of all individuals values = " + sumawartości.ToString();
                    break;

                // roulette
                case 3:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Selection - Roulette wheel method - drawing";

                    // zero the chosen table
                    for (int i = 0; i < 20; i++) people[i].chosen = 0;

                    // we draw 20 numbers from 0 to 99
                    for (int i = 0; i < 20; i++)
                    {
                        int random = rnd.Next(100);
                        int sumofchances = 0;
                        // we check which range the number has fallen into and we sum up the hits
                        for (int j = 0; j < 20; j++)
                        {
                            if ((random >= sumofchances) && (random < (sumofchances + (int)Math.Ceiling(people[j].chances))))
                            {
                                people[j].chosen++;
                            }
                            sumofchances += (int)Math.Ceiling(people[j].chances);
                        }

                    }

                    // display again and show how many times which individual has been chosen
                    Display();
                    o1.Text += "  (" + people[0].chosen.ToString() + " times)";
                    o2.Text += "  (" + people[1].chosen.ToString() + " times)";
                    o3.Text += "  (" + people[2].chosen.ToString() + " times)";
                    o4.Text += "  (" + people[3].chosen.ToString() + " times)";
                    o5.Text += "  (" + people[4].chosen.ToString() + " times)";
                    o6.Text += "  (" + people[5].chosen.ToString() + " times)";
                    o7.Text += "  (" + people[6].chosen.ToString() + " times)";
                    o8.Text += "  (" + people[7].chosen.ToString() + " times)";
                    o9.Text += "  (" + people[8].chosen.ToString() + " times)";
                    o10.Text += "  (" + people[9].chosen.ToString() + " times)";
                    o11.Text += "  (" + people[10].chosen.ToString() + " times)";
                    o12.Text += "  (" + people[11].chosen.ToString() + " times)";
                    o13.Text += "  (" + people[12].chosen.ToString() + " times)";
                    o14.Text += "  (" + people[13].chosen.ToString() + " times)";
                    o15.Text += "  (" + people[14].chosen.ToString() + " times)";
                    o16.Text += "  (" + people[15].chosen.ToString() + " times)";
                    o17.Text += "  (" + people[16].chosen.ToString() + " times)";
                    o18.Text += "  (" + people[17].chosen.ToString() + " times)";
                    o19.Text += "  (" + people[18].chosen.ToString() + " times)";
                    o20.Text += "  (" + people[19].chosen.ToString() + " times)";
                    break;

                // new parent pool
                case 4:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "New parent pool";

                    // temporary list for the new parent pool
                    List<Point5D> tmppeople = new List<Point5D>();

                    // we are generating a new parent pool
                    foreach (Point5D p in people)
                    {
                        int addcount = p.chosen;
                        while (addcount > 0)
                        {
                            // we add to the temporary list as many times as the individual has been chosen
                            tmppeople.Add(new Point5D(p.chromosome, p.weight, p.value, 0, 0, false, false));
                            addcount--;
                        }
                    }

                    // clear the old population list and replace it with the new one saved in tmppeople
                    people.Clear();
                    foreach (Point5D p in tmppeople)
                    {
                        people.Add(new Point5D(p.chromosome, p.weight, p.value, 0, 0, false, false));
                    }

                    // view new parental pool
                    Display();
                    break;

                // crossover checking
                case 5:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Crossing - we check for which pair it will occur";

                    // we associate individuals in pairs as they are arranged in the parental pool (eg 1 with 2, 3 with 4, 5 with 6, etc.)
                    for (int i = 0; i < 20; i += 2)
                    {
                        int random = rnd.Next(100);
                        if (random < (int)(pcrossing * 100.0f))
                        {
                            // we cross
                            people[i].wecross = true;
                            people[i + 1].wecross = true;
                        }
                    }

                    // display again and show where the crossing occurs
                    Display();
                    o1.Text += people[0].wecross ? "  (yes)" : "  (no)";
                    o2.Text += people[1].wecross ? "  (yes)" : "  (no)";
                    o3.Text += people[2].wecross ? "  (yes)" : "  (no)";
                    o4.Text += people[3].wecross ? "  (yes)" : "  (no)";
                    o5.Text += people[4].wecross ? "  (yes)" : "  (no)";
                    o6.Text += people[5].wecross ? "  (yes)" : "  (no)";
                    o7.Text += people[6].wecross ? "  (yes)" : "  (no)";
                    o8.Text += people[7].wecross ? "  (yes)" : "  (no)";
                    o9.Text += people[8].wecross ? "  (yes)" : "  (no)";
                    o10.Text += people[9].wecross ? "  (yes)" : "  (no)";
                    o11.Text += people[10].wecross ? "  (yes)" : "  (no)";
                    o12.Text += people[11].wecross ? "  (yes)" : "  (no)";
                    o13.Text += people[12].wecross ? "  (yes)" : "  (no)";
                    o14.Text += people[13].wecross ? "  (yes)" : "  (no)";
                    o15.Text += people[14].wecross ? "  (yes)" : "  (no)";
                    o16.Text += people[15].wecross ? "  (yes)" : "  (no)";
                    o17.Text += people[16].wecross ? "  (yes)" : "  (no)";
                    o18.Text += people[17].wecross ? "  (yes)" : "  (no)";
                    o19.Text += people[18].wecross ? "  (yes)" : "  (no)";
                    o20.Text += people[19].wecross ? "  (yes)" : "  (no)";
                    break;

                // execute crossing
                case 6:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Crossing - we execute";

                    // we draw a crossing point for each pair subject to crossing
                    for (int i = 0; i < 20; i += 2)
                    {
                        // we draw a position from 1 to 9 because we have 10 bits and count from 0 (count starts from the right)
                        int random = rnd.Next(0, 10);
                        if (people[i].wecross)
                        {
                            int mask = (int)Math.Pow(2.00, random); // 2.00 ^ ranodm, position 1,2,4,8,16,32,64,128,256,512
                            mask--; // mask, the bits that stay on the right side

                            int first = people[i].chromosome;
                            int second = people[i + 1].chromosome;

                            // we leave the right side (what we will corss)
                            first &= mask;
                            second &= mask;

                            // we leave the left side (what does not change) 2^10 - 1 = 1023 (that is 10 bits)
                            people[i].chromosome &= (int)Math.Pow(2.00, 10) - 1 - mask;
                            people[i + 1].chromosome &= (int)Math.Pow(2.00, 10) - 1 - mask;

                            // we cross (we add in reverse what we left on the right side for first and second)
                            people[i].chromosome |= second;
                            people[i + 1].chromosome |= first;

                            // calculate new individuals
                            people[i].weight = CalcWeight((int)people[i].chromosome);
                            people[i].value = CalcValue((int)people[i].chromosome);
                            people[i + 1].weight = CalcWeight((int)people[i + 1].chromosome);
                            people[i + 1].value = CalcValue((int)people[i + 1].chromosome);
                        }
                    }

                    // Display individuals after crossing
                    Display();
                    break;

                // check mutations
                case 7:
                    OpisKroku.Text = "Step " + steps.ToString() + " : " + "Mutation - we check for which individual it occurs";

                    // for all individuals
                    for (int i = 0; i < 20; i++)
                    {
                        int losowa = rnd.Next(100);
                        if (losowa < (int)(pmutation * 100.0f))
                        {
                            // we mutate
                            people[i].wemutate = true;
                        }
                    }

                    // Display again and show where does mutation occur
                    Display();
                    o1.Text += people[0].wemutate ? "  (yes)" : "  (no)";
                    o2.Text += people[1].wemutate ? "  (yes)" : "  (no)";
                    o3.Text += people[2].wemutate ? "  (yes)" : "  (no)";
                    o4.Text += people[3].wemutate ? "  (yes)" : "  (no)";
                    o5.Text += people[4].wemutate ? "  (yes)" : "  (no)";
                    o6.Text += people[5].wemutate ? "  (yes)" : "  (no)";
                    o7.Text += people[6].wemutate ? "  (yes)" : "  (no)";
                    o8.Text += people[7].wemutate ? "  (yes)" : "  (no)";
                    o9.Text += people[8].wemutate ? "  (yes)" : "  (no)";
                    o10.Text += people[9].wemutate ? "  (yes)" : "  (no)";
                    o11.Text += people[10].wemutate ? "  (yes)" : "  (no)";
                    o12.Text += people[11].wemutate ? "  (yes)" : "  (no)";
                    o13.Text += people[12].wemutate ? "  (yes)" : "  (no)";
                    o14.Text += people[13].wemutate ? "  (yes)" : "  (no)";
                    o15.Text += people[14].wemutate ? "  (yes)" : "  (no)";
                    o16.Text += people[15].wemutate ? "  (yes)" : "  (no)";
                    o17.Text += people[16].wemutate ? "  (yes)" : "  (no)";
                    o18.Text += people[17].wemutate ? "  (yes)" : "  (no)";
                    o19.Text += people[18].wemutate ? "  (yes)" : "  (no)";
                    o20.Text += people[19].wemutate ? "  (yes)" : "  (no)";
                    break;

                // execute mutations
                case 8:
                    OpisKroku.Text = "Krok " + steps.ToString() + " : " + "Mutations - we execute";

                    // draw a random mutation point
                    for (int i = 0; i < 20; i++)
                    {
                        // we draw a random mutation point from 0 to 9 because we have 10 bits
                        int random = rnd.Next(10);
                        if (people[i].wemutate)
                        {
                            int position = (int)Math.Pow(2.00, random); // 2.00 ^ random, position 1,2,4,8,16,32,64,128,256,512

                            // we mutate
                            people[i].chromosome ^= position;
                            people[i].weight = CalcWeight((int)people[i].chromosome);
                            people[i].value = CalcValue((int)people[i].chromosome);
                        }
                    }

                    // Display individuals after mutation
                    Display();
                    break;

                case 9:
                    steps = 0;
                    OpisKroku.Text = "Last step : Return to the adaptation function";
                    iterations++;
                    iteracja.Text = "Algorith iteration number : " + iterations.ToString();
                    break;
            }

            // go to next step
            steps++;
        }


        // helper functions
        int CalcWeight(int value)
        {
            int weight = 0;
            // we have 10 items (10 bits)
            if ((value & 512) == 512) weight += (int)items[0].Y;
            if ((value & 256) == 256) weight += (int)items[1].Y;
            if ((value & 128) == 128) weight += (int)items[2].Y;
            if ((value & 64) == 64) weight += (int)items[3].Y;
            if ((value & 32) == 32) weight += (int)items[4].Y;
            if ((value & 16) == 16) weight += (int)items[5].Y;
            if ((value & 8) == 8) weight += (int)items[6].Y;
            if ((value & 4) == 4) weight += (int)items[7].Y;
            if ((value & 2) == 2) weight += (int)items[8].Y;
            if ((value & 1) == 1) weight += (int)items[9].Y;

            return weight;
        }

        int CalcValue(int value)
        {
            int sumofvalues = 0;
            // we have 10 items (10 bits)
            if ((value & 512) == 512) sumofvalues += (int)items[0].Z;
            if ((value & 256) == 256) sumofvalues += (int)items[1].Z;
            if ((value & 128) == 128) sumofvalues += (int)items[2].Z;
            if ((value & 64) == 64) sumofvalues += (int)items[3].Z;
            if ((value & 32) == 32) sumofvalues += (int)items[4].Z;
            if ((value & 16) == 16) sumofvalues += (int)items[5].Z;
            if ((value & 8) == 8) sumofvalues += (int)items[6].Z;
            if ((value & 4) == 4) sumofvalues += (int)items[7].Z;
            if ((value & 2) == 2) sumofvalues += (int)items[8].Z;
            if ((value & 1) == 1) sumofvalues += (int)items[9].Z;

            return sumofvalues;
        }

        void Display()
        {
            // if it is higher than weightmax then it is displayed in red
            if (people[0].weight > weightmax) o1.ForeColor = Color.Red;
            else o1.ForeColor = Color.Black;
            o1.Text = "person 1   : " + Convert.ToString((int)people[0].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[0].weight.ToString() + "   value = " + people[0].value.ToString();
            if (people[1].weight > weightmax) o2.ForeColor = Color.Red;
            else o2.ForeColor = Color.Black;
            o2.Text = "person 2   : " + Convert.ToString((int)people[1].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[1].weight.ToString() + "   value = " + people[1].value.ToString();
            if (people[2].weight > weightmax) o3.ForeColor = Color.Red;
            else o3.ForeColor = Color.Black;
            o3.Text = "person 3   : " + Convert.ToString((int)people[2].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[2].weight.ToString() + "   value = " + people[2].value.ToString();
            if (people[3].weight > weightmax) o4.ForeColor = Color.Red;
            else o4.ForeColor = Color.Black;
            o4.Text = "person 4   : " + Convert.ToString((int)people[3].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[3].weight.ToString() + "   value = " + people[3].value.ToString();
            if (people[4].weight > weightmax) o5.ForeColor = Color.Red;
            else o5.ForeColor = Color.Black;
            o5.Text = "person 5   : " + Convert.ToString((int)people[4].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[4].weight.ToString() + "   value = " + people[4].value.ToString();
            if (people[5].weight > weightmax) o6.ForeColor = Color.Red;
            else o6.ForeColor = Color.Black;
            o6.Text = "person 6   : " + Convert.ToString((int)people[5].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[5].weight.ToString() + "   value = " + people[5].value.ToString();
            if (people[6].weight > weightmax) o7.ForeColor = Color.Red;
            else o7.ForeColor = Color.Black;
            o7.Text = "person 7   : " + Convert.ToString((int)people[6].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[6].weight.ToString() + "   value = " + people[6].value.ToString();
            if (people[7].weight > weightmax) o8.ForeColor = Color.Red;
            else o8.ForeColor = Color.Black;
            o8.Text = "person 8   : " + Convert.ToString((int)people[7].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[7].weight.ToString() + "   value = " + people[7].value.ToString();
            if (people[8].weight > weightmax) o9.ForeColor = Color.Red;
            else o9.ForeColor = Color.Black;
            o9.Text = "person 9   : " + Convert.ToString((int)people[8].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[8].weight.ToString() + "   value = " + people[8].value.ToString();
            if (people[9].weight > weightmax) o10.ForeColor = Color.Red;
            else o10.ForeColor = Color.Black;
            o10.Text = "person 10 : " + Convert.ToString((int)people[9].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[9].weight.ToString() + "   value = " + people[9].value.ToString();
            if (people[10].weight > weightmax) o11.ForeColor = Color.Red;
            else o11.ForeColor = Color.Black;
            o11.Text = "person 11 : " + Convert.ToString((int)people[10].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[10].weight.ToString() + "   value = " + people[10].value.ToString();
            if (people[11].weight > weightmax) o12.ForeColor = Color.Red;
            else o12.ForeColor = Color.Black;
            o12.Text = "person 12 : " + Convert.ToString((int)people[11].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[11].weight.ToString() + "   value = " + people[11].value.ToString();
            if (people[12].weight > weightmax) o13.ForeColor = Color.Red;
            else o13.ForeColor = Color.Black;
            o13.Text = "person 13 : " + Convert.ToString((int)people[12].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[12].weight.ToString() + "   value = " + people[12].value.ToString();
            if (people[13].weight > weightmax) o14.ForeColor = Color.Red;
            else o14.ForeColor = Color.Black;
            o14.Text = "person 14 : " + Convert.ToString((int)people[13].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[13].weight.ToString() + "   value = " + people[13].value.ToString();
            if (people[14].weight > weightmax) o15.ForeColor = Color.Red;
            else o15.ForeColor = Color.Black;
            o15.Text = "person 15 : " + Convert.ToString((int)people[14].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[14].weight.ToString() + "   value = " + people[14].value.ToString();
            if (people[15].weight > weightmax) o16.ForeColor = Color.Red;
            else o16.ForeColor = Color.Black;
            o16.Text = "person 16 : " + Convert.ToString((int)people[15].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[15].weight.ToString() + "   value = " + people[15].value.ToString();
            if (people[16].weight > weightmax) o17.ForeColor = Color.Red;
            else o17.ForeColor = Color.Black;
            o17.Text = "person 17 : " + Convert.ToString((int)people[16].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[16].weight.ToString() + "   value = " + people[16].value.ToString();
            if (people[17].weight > weightmax) o18.ForeColor = Color.Red;
            else o18.ForeColor = Color.Black;
            o18.Text = "person 18 : " + Convert.ToString((int)people[17].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[17].weight.ToString() + "   value = " + people[17].value.ToString();
            if (people[18].weight > weightmax) o19.ForeColor = Color.Red;
            else o19.ForeColor = Color.Black;
            o19.Text = "person 19 : " + Convert.ToString((int)people[18].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[18].weight.ToString() + "   value = " + people[18].value.ToString();
            if (people[19].weight > weightmax) o20.ForeColor = Color.Red;
            else o20.ForeColor = Color.Black;
            o20.Text = "person 20 : " + Convert.ToString((int)people[19].chromosome, 2).PadLeft(10, '0') + "   weight = " + people[19].weight.ToString() + "   value = " + people[19].value.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            w1.Text = "13";
            w2.Text = "6";
            w3.Text = "9";
            w4.Text = "14";
            w5.Text = "12";
            w6.Text = "5";
            w7.Text = "7";
            w8.Text = "3";
            w9.Text = "10";
            w10.Text = "13";
            v1.Text = "4";
            v2.Text = "15";
            v3.Text = "12";
            v4.Text = "8";
            v5.Text = "6";
            v6.Text = "12";
            v7.Text = "3";
            v8.Text = "14";
            v9.Text = "3";
            v10.Text = "7";
            wmax.Text = "78";
            pk.Text = "0,8";
            pm.Text = "0,1";
        }

    }

    // primitive helper class
    public class Point5D
    {
        public int chromosome;
        public int weight;
        public int value;
        public float chances;
        public int chosen;
        public bool wecross;
        public bool wemutate;

        public Point5D(int nchromosome, int nweight, int nvalue, float nchances, int nchosen, bool nwecross, bool nwemutate)
        {
            chromosome = nchromosome;
            weight = nweight;
            value = nvalue;
            chances = nchances;
            chosen = nchosen;
            wecross = nwecross;
            wemutate = nwemutate;
        }
    }

}
