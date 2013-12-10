using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using HelpOverlay.Model;
using System.Windows.Input;
using System.Windows;

namespace HelpOverlay.ViewModel
{
    class ClickyRecordingViewModel : ViewModelBase
    {
        public ClickyRecordingViewModel()
        {
            RecordedClicks = new ObservableCollection<Step>();
            SaveFile = TutorialStorage.LoadTutorial();
        }

        private ObservableCollection<Step> _recordedClicks;
        public ObservableCollection<Step> RecordedClicks
        {
            get { return _recordedClicks; }
            set
            {
                _recordedClicks = value;
                NotifyPropertyChanged("RecordedClicks");
            }
        }

        private ObservableCollection<Tutorial> _tutorials;
        public ObservableCollection<Tutorial> Tutorials
        {
            get { return _tutorials; }
            set
            {
                _tutorials = value;
                NotifyPropertyChanged("Tutorials");
            }
        }

        private TutorialStorage _saveFile;
        public TutorialStorage SaveFile
        {
            get { return _saveFile; }
            set
            {
                _saveFile = value;
                NotifyPropertyChanged("SaveFile");
                Tutorials = new ObservableCollection<Tutorial>(_saveFile.FullTutorials);
            }
        }

        private Tutorial _selectedTutorial;
        public Tutorial SelectedTutorial
        {
            get { return _selectedTutorial; }
            set
            {
                _selectedTutorial = value;
                NotifyPropertyChanged("SelectedTutorial");
            }
        }

        public ICommand Save
        {
            get
            {
                return new RelayCommand((object arg) =>
                    {
                        Tutorial thisTutorial = new Tutorial();
                        thisTutorial.ApplicationName = Application.Current.MainWindow.Title;
                        thisTutorial.Steps = this.RecordedClicks.ToList();
                        thisTutorial.SaveName = Application.Current.MainWindow.Title;
                        
                        TutorialStorage ts = TutorialStorage.LoadTutorial();
                        ts.FullTutorials.Add(thisTutorial);

                        TutorialStorage.SaveTutorial(ts);


                        SaveFile = TutorialStorage.LoadTutorial();
                    });
            }
        }

        public ICommand PlayTutorial
        {
            get
            {
                return new RelayCommand((object arg) =>
                    {
                        TutorialManager.CurrentTutorial = SelectedTutorial;
                        
                        TutorialManager.CurrentTutorial.Begin();
                    });
            }
        }
    }
}
