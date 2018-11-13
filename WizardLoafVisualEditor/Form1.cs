using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing;

namespace WizardLoafVisualEditor
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        //Current issue: Color dialog opens all color panels

        /// <summary>
        /// TODO:
        /// 1. Profile creation
        /// 2. Profile loading
        /// 3.Numeric Sliders
        /// 4. Project creation that links to "engine"
        /// 5. Color wheel 
        /// 6. Curves for exponential values
        /// </summary>


        bool firstReimport = true;

        string tempSourceShaderPath = @"TempFiles\tempPath.txt";

        //General Vars
        string sourceShaderPath;
        System.IFormatProvider cultureUS = new CultureInfo("en-US");

        List<string> lines = new List<string>();
        List<ParameterObject> globalParameterList = new List<ParameterObject>();
        List<ParameterObject> globalIntParameterList = new List<ParameterObject>();
        List<ParameterObject> globalFloatParameterList = new List<ParameterObject>();
        List<ParameterObject> globalColorParameterList = new List<ParameterObject>();
        List<GroupBox> globalGroupBoxes = new List<GroupBox>();
        List<NumericUpDown> globalIntNumerInputs = new List<NumericUpDown>();
        List<NumericUpDown> globalFloatNumerInputs = new List<NumericUpDown>();
        List<Color> globalColorList = new List<Color>();
        List<ColorDialog> globalColorDialogList = new List<ColorDialog>();
        List<Button> globalColorInputButton = new List<Button>();
        List<PictureBox> globalColorBoxes = new List<PictureBox>();



        List<ParameterObject> soloParameterList = new List<ParameterObject>();
        List<ParameterObject> soloIntParameterList = new List<ParameterObject>();
        List<ParameterObject> soloFloatParameterList = new List<ParameterObject>();
        List<ParameterObject> soloColorParameterList = new List<ParameterObject>();
        List<GroupBox> soloGroupBoxes = new List<GroupBox>();
        List<NumericUpDown> soloIntNumerInputs = new List<NumericUpDown>();
        List<NumericUpDown> soloFloatNumerInputs = new List<NumericUpDown>();
        List<Color> soloColorList = new List<Color>();
        List<ColorDialog> soloColorDialogList = new List<ColorDialog>();
        List<Button> soloColorInputButton = new List<Button>();
        List<PictureBox> soloColorBoxes = new List<PictureBox>();



        //Selection and sorting
        void CheckFunction()
        {
            if (firstReimport == false)
            {
                File.WriteAllText(tempSourceShaderPath, sourceShaderPath);
                Application.Restart();
            }

            CheckForParameters();
            SortGlobalParameters();
            FillGlobalListBox();

            SortSoloParameters();
            FillSoloListBox();

            this.Text = sourceShaderPath + " - " + "WizardLoafVisualEditor";
            firstReimport = false;
        }

        void ParseShaderAndFillList()
        {
            using (StreamReader r = new StreamReader(sourceShaderPath))
            {
                string[] lineArray = File.ReadAllLines(sourceShaderPath);
                
                //Search for global parameters
                for (int i = 0; i < lineArray.Length; i++)
                {
                    lines.Add(lineArray[i]);
                }
            }
        }

        void CheckForParameters()
        {
            for(int i = 0; i < lines.Count; i++)
            {
                if(lines[i].Contains("/*"))
                {
                    int charLocation = lines[i].IndexOf("*/", StringComparison.Ordinal);

                    string textToCheck = lines[i].Substring(0, charLocation);

                    if(textToCheck.ToLower().Contains("global"))
                    {
                        if (textToCheck.ToLower().Contains("-int -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "int";

                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;

                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\s(-)?\d*\;");
                            Match valueMatch = varValue.Match(lines[i]);

                            string value = valueMatch.Groups[0].Value.Substring(1, valueMatch.Groups[0].Value.Length - 2);
                            pObj.variableValue = value;

                            globalParameterList.Add(pObj);
                            globalIntParameterList.Add(pObj);
                        }
                        else if (textToCheck.ToLower().Contains("-float -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "float";

                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;

                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\s(-)?\d*(\.\d*)");
                            Match valueMatch = varValue.Match(lines[i]);

                            if(valueMatch.Groups[0].Value == null || valueMatch.Groups[0].Value == "")
                            {
                                MessageBox.Show("Error on line " + (pObj.line + 1) + ": floats not valid, even if the float is a whole number it needs .0 at the end.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                string value = valueMatch.Groups[0].Value.Substring(0, valueMatch.Groups[0].Value.Length - 2);
                                pObj.variableValue = value;


                                globalParameterList.Add(pObj);
                                globalFloatParameterList.Add(pObj);
                            }
                        }
                        else if (textToCheck.ToLower().Contains("-color -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "color";



                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;



                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\((\d*\.\d*),(\s)?(\d*\.\d*)\,(\s)?\d*(\.\d*)?\);");
                            Match valueMatch = varValue.Match(lines[i]);
                            if (valueMatch.Success)
                            {
                                string value = valueMatch.Groups[0].Value;
                                pObj.variableValue = value;
                                globalParameterList.Add(pObj);
                                globalColorParameterList.Add(pObj);
                            }
                            else
                            {
                                MessageBox.Show("Error on line " + (pObj.line + 1) + ": float3 color flags can not contains negative values.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else if (textToCheck.ToLower().Contains("solo"))
                    {
                        if (textToCheck.ToLower().Contains("-int -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "int";

                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;

                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\s(-)?\d*\;");
                            Match valueMatch = varValue.Match(lines[i]);

                            string value = valueMatch.Groups[0].Value.Substring(1, valueMatch.Groups[0].Value.Length - 2);
                            pObj.variableValue = value;

                            soloParameterList.Add(pObj);
                            soloIntParameterList.Add(pObj);
                        }
                        else if (textToCheck.ToLower().Contains("-float -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "float";

                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;

                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\s(-)?\d*(\.\d*)");
                            Match valueMatch = varValue.Match(lines[i]);

                            if (valueMatch.Groups[0].Value == null || valueMatch.Groups[0].Value == "")
                            {
                                MessageBox.Show("Error on line " + (pObj.line + 1) + ": floats not valid, even if the float is a whole number it needs .0 at the end.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                string value = valueMatch.Groups[0].Value.Substring(0, valueMatch.Groups[0].Value.Length - 2);
                                pObj.variableValue = value;


                                soloParameterList.Add(pObj);
                                soloFloatParameterList.Add(pObj);
                            }
                        }
                        else if (textToCheck.ToLower().Contains("-color -"))
                        {
                            ParameterObject pObj = new ParameterObject();
                            pObj.line = i;
                            pObj.variableType = "color";



                            Regex catName = new Regex(@"\-\w*\*");
                            Match catMatch = catName.Match(lines[i]);

                            string cat = catMatch.Groups[0].Value.Substring(1, catMatch.Groups[0].Value.Length - 2);
                            pObj.categoryName = cat;



                            //To get variable name
                            Regex varName = new Regex(@"\w*?\s(?<!=)=(?!=)");
                            Match nameMatch = varName.Match(lines[i]);

                            string name = nameMatch.Groups[0].Value.Substring(0, nameMatch.Groups[0].Value.Length - 2);
                            pObj.variableName = name;

                            Regex varValue = new Regex(@"\((\d*\.\d*),(\s)?(\d*\.\d*)\,(\s)?\d*(\.\d*)?\);");
                            Match valueMatch = varValue.Match(lines[i]);
                            if (valueMatch.Success)
                            {
                                string value = valueMatch.Groups[0].Value;
                                pObj.variableValue = value;
                                soloParameterList.Add(pObj);
                                soloColorParameterList.Add(pObj);
                            }
                            else
                            {
                                MessageBox.Show("Error on line " + (pObj.line + 1) + ": float3 color flags can not contains negative values.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }

        void SortGlobalParameters()
        {
            List<string> tempStringList = new List<string>();
            int counter = 1;

            //Create list of unique categories
            for (int i = 0; i < globalParameterList.Count; i++)
            {
                if(tempStringList.Contains(globalParameterList[i].categoryName) == false)
                {
                    tempStringList.Add(globalParameterList[i].categoryName);
                }
            }

            for(int i = 0; i < tempStringList.Count; i++)
            {
                globalGroupBoxes.Add(CreateGlobalGroupBoxControl(tempStringList[i], i));
                foreach(ParameterObject pObject in globalParameterList)
                {
                    if (pObject.categoryName == tempStringList[i])
                    {
                        if (pObject.variableType == "int")
                        {
                            CreateIntControl(globalGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                        else if(pObject.variableType == "float")
                        {
                            CreateFloatControl(globalGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                        else if (pObject.variableType == "color")
                        {
                            CreateColorControl(globalGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                    }
                }
                counter = 1;
            }
        }

        void SortSoloParameters()
        {
            List<string> tempStringList = new List<string>();
            int counter = 1;

            //Create list of unique categories
            for (int i = 0; i < soloParameterList.Count; i++)
            {
                if (tempStringList.Contains(soloParameterList[i].categoryName) == false)
                {
                    tempStringList.Add(soloParameterList[i].categoryName);
                }
            }

            for (int i = 0; i < tempStringList.Count; i++)
            {
                soloGroupBoxes.Add(CreateSoloGroupBoxControl(tempStringList[i], i));
                foreach (ParameterObject pObject in soloParameterList)
                {
                    if (pObject.categoryName == tempStringList[i])
                    {
                        if (pObject.variableType == "int")
                        {
                            CreateIntControl(soloGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                        else if (pObject.variableType == "float")
                        {
                            CreateFloatControl(soloGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                        else if (pObject.variableType == "color")
                        {
                            CreateColorControl(soloGroupBoxes[i], pObject, counter);
                            counter++;
                        }
                    }
                }
                counter = 1;
            }
        }

        void FillGlobalListBox()
        {
            for (int i = 0; i < globalGroupBoxes.Count; i++)
            {
                listBox1.Items.Add(globalGroupBoxes[i].Name);
            }
        }

        void FillSoloListBox()
        {
            for (int i = 0; i < soloGroupBoxes.Count; i++)
            {
                listBox2.Items.Add(soloGroupBoxes[i].Name);
            }
        }


        void CheckIfNewFileIsOpened()
        {
            if (File.Exists(tempSourceShaderPath))
            {
                using (StreamReader r = new StreamReader(tempSourceShaderPath))
                {
                    sourceShaderPath = r.ReadToEnd();
                    ParseShaderAndFillList();
                    CheckFunction();
                    r.Close();
                }

                File.Delete(tempSourceShaderPath);
            }
        }


        //Saving
        void SaveGlobalChanges()
        {
            int intValueCounter = 0;
            int floatValueCounter = 0;
            int colorValueCounter = 0;


            //Check ints
            for (int i = 0; i < lines.Count; i++)
            {
                if(lines[i] != "")
                {
                    //INT CHECK
                    if (lines[i].Contains(" -int -"))
                    {
                        lines[i] = Regex.Replace(lines[i], @"\s(-)?\d*;", " " + globalIntNumerInputs[intValueCounter].Value.ToString(cultureUS) + ";");

                        intValueCounter++;
                    }

                    //FLOAT CHECK
                    else if (lines[i].Contains(" -float -"))
                    {
                        if (globalFloatNumerInputs[floatValueCounter].Value.ToString(cultureUS).Contains(".") == false || globalFloatNumerInputs[floatValueCounter].Value.ToString(cultureUS).Contains(",") == false)
                        {
                            lines[i] = Regex.Replace(lines[i], @"\s(\-)?\d*\.\d*;", " " + globalFloatNumerInputs[floatValueCounter].Value.ToString(cultureUS) + ".0;");
                        }
                        else
                        {
                            lines[i] = Regex.Replace(lines[i], @"\s(\-)?\d*\.\d*;", " " + globalFloatNumerInputs[floatValueCounter].Value.ToString(cultureUS) + ";");
                        }

                        floatValueCounter++;
                    }

                    //COLOR CHECK
                    else if (lines[i].Contains(" -color -"))
                    {
                        float col_r = (float)decimal.Parse(globalColorList[colorValueCounter].R.ToString(cultureUS)) / 255;
                        float col_g = (float)decimal.Parse(globalColorList[colorValueCounter].G.ToString(cultureUS)) / 255;
                        float col_b = (float)decimal.Parse(globalColorList[colorValueCounter].B.ToString(cultureUS)) / 255;

                        float roundR = (float)Math.Round(col_r, 3);
                        float roundG = (float)Math.Round(col_g, 3);
                        float roundB = (float)Math.Round(col_b, 3);

                        string finalR = roundR.ToString();
                        if (finalR.Contains(",") == false) 
                        {
                            finalR += ".0";
                        }

                        string finalG = roundG.ToString();
                        if (finalG.Contains(",") == false)
                        {
                            finalG += ".0";
                        }

                        string finalB = roundB.ToString();
                        if (finalB.Contains(",") == false)
                        {
                            finalB += ".0";
                        }

                        string pattern = @"\((\d*\.\d*),(\s)?(\d*\.\d*)\,(\s)?(\d*\.\d*)\);";
                        string replaceString = "(" + finalR.Replace(",", ".") + ", " + finalG.Replace(",", ".") + ", " + finalB.Replace(",", ".") + ");";

                        lines[i] = Regex.Replace(lines[i], pattern, replaceString);

                        colorValueCounter++;
                    }
                }
            }

            //Saving to the .fx file
            File.WriteAllLines(sourceShaderPath, lines);
            MessageBox.Show("Save succesful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //Profiles
        void SaveProfile()
        {
            using (StreamWriter sr = new StreamWriter("Profiles/Profile.txt"))
            {
                for(int i = 0; i < globalParameterList.Count; i++)
                {
                    string line = lines[globalParameterList[i].line];
                    sr.WriteLine(line);
                }
                MessageBox.Show("Done");
            }
        }


        //Misc
        private void button_Click(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < globalColorInputButton.Count; i++)
            {
                DialogResult result = globalColorDialogList[i].ShowDialog();

                if(result == DialogResult.OK)
                {
                    globalColorBoxes[i].BackColor = globalColorDialogList[i].Color;
                    globalColorList[i] = globalColorDialogList[i].Color;
                    return;
                }
            }
        }


        //Control creation functions
        GroupBox CreateGlobalGroupBoxControl(string groupName, int aControlNumber)
        {
            GroupBox g = new GroupBox();
            g.Location = new Point(15, 15);
            g.Size = new Size(475, 300);
            g.Name = groupName;
            g.Text = groupName;
            g.ForeColor = Color.White;

            panel3.Controls.Add(g);

            return g;
        }

        GroupBox CreateSoloGroupBoxControl(string groupName, int aControlNumber)
        {
            GroupBox g = new GroupBox();
            g.Location = new Point(15, 15);
            g.Size = new Size(475, 300);
            g.Name = groupName;
            g.Text = groupName;
            g.ForeColor = Color.White;

            panel5.Controls.Add(g);

            return g;
        }


        void CreateIntControl(GroupBox aGroupBox, ParameterObject aParameterObject, int aControlNumber)
        {
            Label l = new Label();
            l.Name = aParameterObject.variableName + "textbox" + aControlNumber;
            l.Location = new Point(15, 30 * aControlNumber);
            l.Text = aParameterObject.variableName;
            l.ForeColor = Color.White;


            NumericUpDown n = new NumericUpDown();
            n.Name = aParameterObject.variableName + "numericInput" + aControlNumber;
            n.Location = new Point(140, 30 * aControlNumber);
            n.Minimum = -10000;
            n.Maximum = 10000;
            n.Value = Int32.Parse(aParameterObject.variableValue);


            aGroupBox.Controls.Add(l);
            aGroupBox.Controls.Add(n);

            globalIntNumerInputs.Add(n);
        }

        void CreateFloatControl(GroupBox aGroupBox, ParameterObject aParameterObject, int aControlNumber)
        {
            Label l = new Label();
            l.Name = aParameterObject.variableName + "textbox" + aControlNumber;
            l.Location = new Point(15, 30 * aControlNumber);
            l.Text = aParameterObject.variableName;
            l.ForeColor = Color.White;

            NumericUpDown n = new NumericUpDown();
            n.Name = aParameterObject.variableName + "numericInput" + aControlNumber;
            n.Location = new Point(140, 30 * aControlNumber);
            n.DecimalPlaces = 2;
            n.Minimum = -10000;
            n.Maximum = 10000;
            n.Value = decimal.Parse(aParameterObject.variableValue, cultureUS);

            aGroupBox.Controls.Add(l);
            aGroupBox.Controls.Add(n);
            globalFloatNumerInputs.Add(n);
        }

        void CreateColorControl(GroupBox aGroupBox, ParameterObject aParameterObject, int aControlNumber)
        {
            Label l = new Label();
            l.Name = aParameterObject.variableName + "textbox" + aControlNumber;
            l.Location = new Point(15, 30 * aControlNumber);
            l.Text = aParameterObject.variableName;
            l.ForeColor = Color.White;

            ColorDialog cd = new ColorDialog();
            Color c = new Color();

            //Read color from line
            Regex colorRGB = new Regex(@"\((\d*\.\d*),(\s)?(\d*\.\d*)\,(\s)?(\d*(\.\d*)?)\);");
            Match colorMatch = colorRGB.Match(aParameterObject.variableValue);

            float col_r = float.Parse(colorMatch.Groups[1].Value, CultureInfo.InvariantCulture) * 255;
            float col_g = float.Parse(colorMatch.Groups[3].Value, CultureInfo.InvariantCulture) * 255;
            float col_b = float.Parse(colorMatch.Groups[5].Value, CultureInfo.InvariantCulture) * 255;

            int channel_r = (int)Math.Round(col_r, 0);
            int channel_g = (int)Math.Round(col_g, 0);
            int channel_b = (int)Math.Round(col_b, 0);

            c = Color.FromArgb(255, channel_r, channel_g, channel_b);


            PictureBox p = new PictureBox();
            p.Size = new Size(60, 20);
            p.Location = new Point(140, 30 * aControlNumber);
            p.BackColor = c;
            p.BorderStyle = BorderStyle.FixedSingle;

            Button b = new Button();
            b.Size = new Size(80, 20);
            b.Location = new Point(210, 30 * aControlNumber);
            b.BackColor = Color.White;
            b.Text = "Pick Color";
            b.ForeColor = Color.Black;



            globalColorList.Add(c);
            globalColorInputButton.Add(b);
            globalColorBoxes.Add(p);
            globalColorDialogList.Add(cd);

            aGroupBox.Controls.Add(l);
            aGroupBox.Controls.Add(p);
            aGroupBox.Controls.Add(b);

            b.Click += button_Click;

        }


        //UI functions
        private void selectShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "fx files (*.fx)|*.fx|hlsl files (*.hlsl)|*.hlsl";


            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                sourceShaderPath = choofdlog.FileName;
                ParseShaderAndFillList();
                CheckFunction();
                return;
            }
            else
            {
                MessageBox.Show("No file was selected.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckIfNewFileIsOpened();
            Application.OpenForms["Form1"].BringToFront();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem == null)
            {
                return;
            }
            for(int i = 0; i < globalGroupBoxes.Count; i++)
            {
                globalGroupBoxes[i].Visible = false;
            }

            for(int i = 0; i < globalGroupBoxes.Count; i++)
            {
                if(globalGroupBoxes[i].Name.ToLower() == listBox1.SelectedItem.ToString().ToLower())
                {
                    globalGroupBoxes[i].Visible = true;
                    return;
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null)
            {
                return;
            }
            for (int i = 0; i < soloGroupBoxes.Count; i++)
            {
                soloGroupBoxes[i].Visible = false;
            }

            for (int i = 0; i < globalGroupBoxes.Count; i++)
            {
                if (soloGroupBoxes[i].Name.ToLower() == listBox2.SelectedItem.ToString().ToLower())
                {
                    soloGroupBoxes[i].Visible = true;
                    return;
                }
            }
        }

        private void applyProfileToEngineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(sourceShaderPath != null)
            {
                SaveGlobalChanges();
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProfile();
        }
    }

    public class ParameterObject
    {
        public int line;
        public string variableType;
        public string categoryName;
        public string variableName;
        public string variableValue;
    }
}