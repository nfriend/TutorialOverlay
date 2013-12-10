using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HelpOverlay
{
    public interface ITutorial
    {
        List<Step> Steps { get; set; }
        string ApplicationName { get; set; }
        string SaveName { get; set; }
        void Begin();
    }

    [Serializable]
    public class Tutorial : ITutorial
    {
        public Tutorial() { }

        public Tutorial(string applicationName, string saveName)
        {
            ApplicationName = applicationName;
            SaveName = saveName;
            Steps = new List<Step>();
        }

        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName; }
            set
            {
                _applicationName = value;
            }
        }

        private string _saveName;
        public string SaveName
        {
            get { return _saveName; }
            set
            {
                _saveName = value;
            }
        }

        private List<Step> _steps;
        public List<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    _steps = new List<Step>();
                }
                return _steps;
            }
            set
            {
                _steps = value;
            }
        }

        private Step _currentStep;
        public Step CurrentStep
        {
            get { return _currentStep; }
            set { _currentStep = value; }
        }
        

        public void Begin()
        {
            if (TutorialManager.Overlay != null && Steps.Count > 0)
            {
                TutorialManager.CurrentTutorial = this;
                this.CurrentStep = Steps[0];
                TutorialManager.Overlay.UpdateOverlay();
                VisualStateManager.GoToElementState(TutorialManager.Overlay, "Shown", true);
            }
        }

        public ICommand GoToNextStep
        {
            get
            {
                return new RelayCommand((Action) =>
                {
                    if (TutorialManager.Overlay != null)
                    {
                        if (Steps.IndexOf(CurrentStep) == Steps.Count - 1)
                        {
                            VisualStateManager.GoToElementState(TutorialManager.Overlay, "Hidden", true);
                            TutorialManager.CurrentTutorial.CurrentStep = null;
                            TutorialManager.CurrentTutorial = null;
                        }
                        else
                        {
                            CurrentStep = Steps[Steps.IndexOf(CurrentStep) + 1];
                            TutorialManager.Overlay.UpdateOverlay();
                        }
                    }
                });
            }
        }
    }
}
