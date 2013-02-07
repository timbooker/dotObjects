
namespace dotObjects.Samples.ScrumManager.Developing
{
    public class Impediment
    {
        private string title;
        private string description;

        internal Impediment(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}