using System;
using System.Collections.Generic;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    /// <summary>
    /// The Process RESTfull URI class.
    /// </summary>
    [Serializable]
    public class ProcessURI
    {
        /// <summary>
        /// The URI separator, "/".
        /// </summary>
        public const string Separator = "/";

        /// <summary>
        /// 
        /// </summary>
        public ProcessURI()
        {
        }

        private ProcessURI(ProcessBehavior behavior)
        {
            Behavior = behavior;
            Argument = ProcessArgument.Parse(behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="behavior"></param>
        public ProcessURI(string entity, ProcessBehavior behavior)
            : this(behavior)
        {
            Entity = entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="behavior"></param>
        /// <param name="arguments"></param>
        public ProcessURI(string entity, ProcessBehavior behavior, params object[] arguments)
            : this(entity, behavior)
        {
            var args = new List<string>();
            Array.ForEach(arguments, a => { if (a != null) args.Add(a.ToString()); });
            Argument = ProcessArgument.Parse(behavior, args.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="behavior"></param>
        /// <param name="argument"></param>
        public ProcessURI(string entity, ProcessBehavior behavior, ProcessArgument argument) : this(entity, behavior)
        {
            Argument = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Entity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessBehavior Behavior { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessArgument Argument { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessURI PreviousURI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Extension { get; set; }
        
        /// <summary>
        /// The index Process URI
        /// </summary>
        public static ProcessURI Index
        {
            get { return new ProcessURI(ProcessBehavior.Index); }
        }

        /// <summary>
        /// Parse the specified RESTfull string into a ProcessURI's instance.
        /// </summary>
        /// <param name="value">A valid RESTfull string.</param>
        /// <returns>A ProcessURI's instance.</returns>
        public static ProcessURI Parse(string value)
        {
            value = string.IsNullOrEmpty(value) ? "index" : value;

            if (value.ToLower().Equals("default") || value.ToLower().Equals("index"))
                return new ProcessURI(ProcessBehavior.Index);

            var arguments = new List<string>(value.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries));

            if (arguments.Count < 2)
                throw new ArgumentException("Invalid URI! It must contains the Entity and Behaviors identifiers.");

            if(arguments[0].ToLower().Equals("session") && arguments.Count > 3)
            {
                var argument = ProcessArgument.Parse(ProcessBehavior.Session, arguments.GetRange(1, arguments.Count - 1).ToArray());
                return new ProcessURI(ProcessBehavior.Session){ Argument = argument };
            }

            var model = arguments[0];

            var behavior = (ProcessBehavior)Enum.Parse(typeof(ProcessBehavior), arguments[1]);

            return (arguments.Count > 2)
                       ? new ProcessURI(model, behavior, arguments.GetRange(2, arguments.Count - 2).ToArray())
                       : new ProcessURI(model, behavior);
        }

        public override bool Equals(object obj)
        {
            return ToString().Equals(obj.ToString());
        }

        public override string ToString()
        {
            string arg = Argument != null ? Argument.ToString() : string.Empty;
            return Entity + Separator + Behavior + (string.IsNullOrEmpty(arg) ? string.Empty : Separator + arg);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result*397) ^ ((Argument != null) ? Argument.GetHashCode() : 1);
                result = (result*397) ^ ((!string.IsNullOrEmpty(Entity)) ? Entity.GetHashCode() : 1);
                result = (result*397) ^ Behavior.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ProcessURI left, ProcessURI right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ProcessURI left, ProcessURI right)
        {
            return !Equals(left, right);
        }
    }
}