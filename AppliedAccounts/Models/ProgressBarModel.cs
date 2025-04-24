namespace AppliedAccounts.Models
{
    public class ProgressBarModel
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double NowValue { get; set; }
        public bool IsAnimated { get; set; }
        public bool IsStriped { get; set; }


        #region Constructors
        public ProgressBarModel()
        {
        
        }

        public ProgressBarModel(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            NowValue = 0;
            IsAnimated = false;
            IsStriped = false;
        }


        public ProgressBarModel(double minValue, double maxValue, double nowValue, bool isAnimated, bool isStriped)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            NowValue = nowValue;
            IsAnimated = isAnimated;
            IsStriped = isStriped;
        }
        #endregion

        #region Get Anumated and Striped
        public string GetAnumated()
        {
            if(IsAnimated)
            {
                return "progress-bar-animated";
            }
            else
            {
                return "";
            }
        }

        public string GetStriped()
        {
            if (IsStriped)
            {
                return "progress-bar-striped";
            }
            else
            {
                return "";
            }
        }
        #endregion

    }


}
