using System;
using System.Collections.Generic;
using dotObjects.Samples.ScrumManager.Developing;

namespace dotObjects.Samples.ScrumManager.Planning
{
    public class Iteration
    {
        private string name;
        private string goals;
        private DateTime start;
        private DateTime end;
        private List<BacklogItem> stories;

        public Iteration(string name, DateTime start, DateTime end, List<Task> tasks)
        {
            Name = name;
            Start = start;
            End = end;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Goals
        {
            get { return goals; }
            set { goals = value; }
        }

        public DateTime Start
        {
            get { return start; }
            set { start = value; }
        }

        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }

        public List<BacklogItem> Stories
        {
            get
            {
                if (stories == null)
                    stories = new List<BacklogItem>();
                return stories;
            }
            set
            {
                stories = value;
            }
        }
    }
}