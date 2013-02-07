using System.Collections.Generic;
using dotObjects.Samples.ScrumManager.Planning;

namespace dotObjects.Samples.ScrumManager.Developing
{
    public class Task
    {
        private BacklogItem backlogItem;
        private string title;
        private TaskStatus status;
        private double estimated;
        private List<Impediment> impediments;
        private Member responsible;

        internal Task(BacklogItem backlogItem, string title, double estimated)
        {
            Title = title;
            Estimated = estimated;
        }

        public BacklogItem BacklogItem
        {
            get { return backlogItem; }
            set { backlogItem = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public TaskStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public double Estimated
        {
            get { return estimated; }
            set { estimated = value; }
        }

        public List<Impediment> Impediments
        {
            get
            {
                if (impediments == null)
                    impediments = new List<Impediment>();
                return impediments;
            }
            set { impediments = value; }
        }

        public Member Responsible
        {
            get { return responsible; }
            set { responsible = value; }
        }

        public void ItsMine(Member member)
        {
            if (member == null)
                Responsible = member;
        }

        public override string ToString()
        {
            return Title + " (" + Estimated + ")";
        }
    }
}