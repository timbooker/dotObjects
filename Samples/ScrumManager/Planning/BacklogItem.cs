using System;
using System.Collections.Generic;
using dotObjects.Samples.ScrumManager.Developing;

namespace dotObjects.Samples.ScrumManager.Planning
{
    [Serializable]
    public class BacklogItem
    {
        private string name;
        private string description;
        private int priority;
        private Iteration iteration;
        private List<Task> tasks;

        public BacklogItem(string name, string description, int priority)
        {
            Name = name;
            Description = description;
            Priority = priority;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public Iteration Iteration
        {
            get { return iteration; }
            internal set { iteration = value; }
        }

        internal List<Task> Tasks
        {
            get
            {
                if (tasks == null)
                    tasks = new List<Task>();
                return tasks;
            }
            set { tasks = value; }
        }

        public void Move(Iteration iteration)
        {
            Iteration.Stories.Remove(this);
            iteration.Stories.Add(this);
            Iteration = iteration;
        }

        public override string ToString()
        {
            return Priority + " - " + Name;
        }
    }
}