using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HelpOverlay.Model
{
    [Serializable]
    public class TutorialStorage
    {
        public TutorialStorage()
        {
            FullTutorials = new List<Tutorial>();
            Step s = new Step();
            TypeIndexAssociation tia = new TypeIndexAssociation();
            s.Path.Add(tia);
            Tutorial t = new Tutorial();
            t.Steps.Add(s);
            //FullTutorials.Add(new Tutorial("App10", "My Tutorial 1"));
            //FullTutorials.Add(new Tutorial("App11", "My Tutorial 2"));
            //FullTutorials.Add(new Tutorial("App12", "My Tutorial 1"));
            //FullTutorials.Add(new Tutorial("App12", "My Tutorial 2"));
            //FullTutorials.Add(new Tutorial("App13", "My Tutorial 3"));
        }

        private static string _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string _saveFileName = "tutorialSave.xml";
        public List<Tutorial> FullTutorials { get; set; }

        public static void SaveTutorial(TutorialStorage ts)
        {
            string pathToSave = Path.Combine(_path, _saveFileName);
            Type t = typeof(TutorialStorage);
            Type t2 = ts.GetType();
            XmlSerializer serializer = new XmlSerializer(typeof(TutorialStorage));//ts.GetType());
            using (FileStream fs = new FileStream(pathToSave, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serializer.Serialize(fs, ts);
            }
        }

        public static TutorialStorage LoadTutorial()
        {
            string pathToLoad = Path.Combine(_path, _saveFileName);

            if (!File.Exists(pathToLoad))
            {
                return new TutorialStorage();
            }

            TutorialStorage ts = new TutorialStorage();
            XmlSerializer deserializer = new XmlSerializer(ts.GetType());
            ts = null;
            using (FileStream fs = new FileStream(pathToLoad, FileMode.Open, FileAccess.Read))
            {
                ts = (TutorialStorage)deserializer.Deserialize(fs);
            }

            foreach(Tutorial t in ts.FullTutorials)
            {
                //if (t.Steps.Count > 0)
                //{
                //}
                //else
                //{
                    foreach (Step s in t.Steps)
                    {
                        foreach (String str in s.StringPath)
                        {
                            string[] array = str.Split('|');
                            s.Path.Add(new TypeIndexAssociation() { ElementType = Type.GetType(array[0], true), Index = Int32.Parse(array[1]) });
                        }
                    }
                //}
            }

            return ts;
        }
    }
}
