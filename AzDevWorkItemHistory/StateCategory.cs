using LanguageExt;

namespace WorkItemHistory
{
    public class StateCategory : Record<StateCategory>
    {
        public static StateCategory Proposed = new StateCategory(nameof(Proposed));
        public static StateCategory InProgress = new StateCategory(nameof(InProgress));
        public static StateCategory Resolved = new StateCategory(nameof(Resolved));
        public static StateCategory Completed = new StateCategory(nameof(Completed));
        public static StateCategory Removed = new StateCategory(nameof(Removed));
        public static StateCategory Unknown(string state) => new StateCategory(state);

        public string CategoryName;

        private StateCategory(string categoryName)
        {
            CategoryName = categoryName;
        }

        public static StateCategory FromString(string category)
        {
            return new StateCategory(category);
        }
    }
}