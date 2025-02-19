﻿using CityChooserAI.Model;
using CityChooserAI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using CityChooserAI.View;

namespace CityChooserAI.ViewModel
{
    internal class ResultsPanel : BaseViewModel
    {
        public MainPanel MainPanel { get; set; }
        public ResultsPanel(MainPanel obj)
        {
            MainPanel = obj;
        }
        #region Attributes
        private List<City> _cityList = new List<City>();
        private List<City> _tmpList;
        private ObservableCollection<string> _resultCityList = new ObservableCollection<string>();
        private string _selectedResultCity;
        #endregion
        #region Getters & setters
        public List<City> cityList
        {
            get { return _cityList; }
            set { _cityList = value; }
        }
        public List<City> tmpList
        {
            get { return _tmpList; }
            set { _tmpList = value; }
        }
        public ObservableCollection<string> resultCityList
        {
            get { return _resultCityList; }
            set 
            { 
                _resultCityList = value;
                OnPropertyChanged(nameof(resultCityList));
            }
        }
        public string selectedResultCity
        {
            get { return _selectedResultCity; }
            set
            {
                _selectedResultCity = value;
                OnPropertyChanged(nameof(resultCityList));
            }
        }
        #endregion
        #region Methods
        public void checkClick(object sender)
        {
            if (MainPanel.chosenAttributes.Count < 3 || MainPanel.chosenAttributes.Count > 6)
            {
                MessageBox.Show("Invalid amount of parameters!");
                return;
            }

            resultCityList.Clear();
            cityList = new List<City>();
            using (var reader = new StreamReader(@"QualityOfLifeDataFrame.csv")) // Add the .csv file to Relase folder!
            {
                List<string> P1 = new List<string>(); // Chosen properties values
                List<string> P2 = new List<string>();
                List<string> P3 = new List<string>();
                List<string> P4 = new List<string>();
                List<string> P5 = new List<string>();
                List<string> P6 = new List<string>();
                List<int> AttributesIndex = new List<int>();

                List<string> CityNames = new List<string>();
                List<decimal> Score = new List<decimal>();
                List<string[]> valuesOfAllCities = new List<string[]>();
                List<string> valuesOfSelectedCities = new List<string>();

                foreach (var attr in MainPanel.chosenAttributes.ToArray())
                {
                    AttributesIndex.Add(Array.IndexOf(Attributes.attributes, attr) + 4); // Get the indexes of chosen columns, ignore first 4 with names
                }

                decimal P1_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));
                decimal P2_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));
                decimal P3_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));
                decimal P4_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));
                decimal P5_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));
                decimal P6_ratio = new decimal((double)(1m / MainPanel.chosenAttributes.Count));

                reader.ReadLine(); // skip first line with column description
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // Not Worldwide and not chosen continent
                    if ("Worldwide" != MainPanel.selectedContinent && values[3] != MainPanel.selectedContinent) continue;

                    // Add if it's Worldwide OR if it isn't Worldwide but the chosen continent
                    if ("Worldwide" == MainPanel.selectedContinent 
                        || ("Worldwide" != MainPanel.selectedContinent && values[3] == MainPanel.selectedContinent))
                    {
                        valuesOfAllCities.Add(values);

                        P1.Add(values[AttributesIndex[0]]);
                        P2.Add(values[AttributesIndex[1]]);
                        P3.Add(values[AttributesIndex[2]]);

                        switch (MainPanel.chosenAttributes.Count)
                        {
                            case 4:
                                P4.Add(values[AttributesIndex[3]]);
                                break;
                            case 5:
                                P4.Add(values[AttributesIndex[3]]);
                                P5.Add(values[AttributesIndex[4]]);
                                break;
                            case 6:
                                P4.Add(values[AttributesIndex[3]]);
                                P5.Add(values[AttributesIndex[4]]);
                                P6.Add(values[AttributesIndex[5]]);
                                break;
                            default:
                                break;
                        }
                        CityNames.Add(values[1]);
                    }
                }
                reader.Close();

                for(int i = 0; i < CityNames.Count; i++) //Calculate score
                {
                    CultureInfo culture = new CultureInfo("en-US"); // for dots in values
                    decimal score = (Convert.ToDecimal(P1[i], culture) * P1_ratio) 
                        + (Convert.ToDecimal(P2[i], culture) * P2_ratio)
                            + (Convert.ToDecimal(P3[i], culture) * P3_ratio);

                    switch (MainPanel.chosenAttributes.Count)
                    {
                        case 3:
                            Score.Add(score);
                            break;
                        case 4:
                            Score.Add(score + (Convert.ToDecimal(P4[i], culture) * P4_ratio));
                            break;
                        case 5:
                            Score.Add(score + (Convert.ToDecimal(P4[i], culture) * P4_ratio) 
                                    + (Convert.ToDecimal(P5[i], culture) * P5_ratio));
                            break;
                        case 6:
                            Score.Add(score + (Convert.ToDecimal(P4[i], culture) * P4_ratio) 
                                    + (Convert.ToDecimal(P5[i], culture) * P5_ratio) 
                                        + (Convert.ToDecimal(P6[i], culture) * P6_ratio));
                            break;
                        default:
                            break;
                    }
                }
                var TOP5 = Score.OrderByDescending(x => x).Take(5).ToArray(); //Get Top5 scores

                for(int i = 0; i < 5; i++) //Get names & values of attributes of Top5 cities
                {
                    var index = Score.IndexOf(TOP5[i]);
                    cityList.Add(new City(valuesOfAllCities.ElementAt(index)));
                    resultCityList.Add(CityNames[index]);
                }
            }
        }
        public void singleCityData(object sender)
        {
            if (selectedResultCity == null) return;

            int selectedIndex = resultCityList.IndexOf(selectedResultCity);
            OutputDataWindow ODW = new OutputDataWindow(cityList[selectedIndex]);
            ODW.Show();
        }
        #endregion
    }
}
