using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PsychComponent
{
    public enum StaircaseTypeEnum
    {
        // by using these values its easy to derive actual STCUp and STCDown parameters implicitly
        st1up_5down = 15,
        st1up_4down = 14,
        st1up_3down = 13,
        st1up_2down = 12,
        st1up_1down = 11,
        st2up_1down = 21,
        st3up_1down = 31,
        st4up_1down = 41,
        st5up_1down = 51
    }
    public enum StepSizeTypeEnum
    {
        ScaledStimVal,
        ScaledStepSize,
        FixedStepSize
    }
    public enum IntervalTypeEnum
    {
        i1AFC = 1,
        i2AFC = 2,
        i3AFC = 3,
        i4AFC = 4,
        i5AFC = 5
    }

    public enum DistributionTypeEnum
    {
        Flat,
        Gaussian
    }

    public enum ProbitTypeEnum
    {
        None,
        Zero_100,
        Fifty_100
    }


    public class UserResponse
    {
        public int SelectedChoice;
        public double ResponseLatency = 0;
        public string UserInput = "";
    }


    public class TrialInstance
    {
        public int StimLevel;
        public int Interval;
        public int Condition;
        public List<string> FixedParams;
        public List<double> RandomParams;
        public double StimVal;
		public int TrialNumber;

        protected TrialInstance()
        {
            // initialise parameter collections
            FixedParams = new List<string>();
            RandomParams = new List<double>();
        }

        public string GetFixedParamsDetails()
        {
            StringBuilder sb;
            string s = "";
            if (this.FixedParams.Count > 0)
            {
                sb = new StringBuilder();
                sb.AppendLine("Fixed parameters:");
                for (int i = 0; i < FixedParams.Count; i++)
                {
                    sb.Append(FixedParams[i] + "\t");
                }
                sb.AppendLine();
                s = sb.ToString();
            }
            return s;


        }

        public string GetRandomParamsDetails()
        {
            StringBuilder sb;
            string s = "";
            if (this.RandomParams.Count > 0)
            {
                sb = new StringBuilder();
                sb.AppendLine("Random parameters:");
                for (int i = 0; i < RandomParams.Count; i++)
                {
                    sb.Append(RandomParams[i] + "\t");
                }
                sb.AppendLine();
                s = sb.ToString();
            }
            return s;

        }
    }

    public class StairCaseTrialInstance : TrialInstance
    {
        public RuntimeStaircase RuntimeStaircase;
    }

    public class ConstantTrialInstance : TrialInstance
    {
        public int InitialIndex = -1;

    }

    public enum StaircaseStatusEnum
    {
        NotStarted,
        Active,
        EndedMaxTrials,
        EndedMaxReversals,
        EndedMaxUpperBoundary,
        EndedMaxLowerBoundary
    }

    public enum PsychComponentStatusEnum
    {
        NotStarted,
        Active,
        Done
    }

    public enum StepTypeEnum
    {
        Start,
        NoChange,
        StepUp,
        StepDown
    }

    public interface IParameter
    {
        string Name
        {
            get;
            set;
        }
    }

    public class Parameter : IParameter
    {
        private string _Name = "";

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

    }

    public class FixedParameter : Parameter
    {
        private List<string> _Levels;


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public List<string> Levels
        {
            get
            {
                return _Levels;
            }
        }


        public FixedParameter()
        {
            Name = "FixedParameter"; //QU: should we keep a count of these and automatically suffix new ones?

            _Levels = new List<string>();
        }

    }

    public class RandomParameter : Parameter
    {
        private DistributionTypeEnum _DistributionType;

        [Browsable(false)]
        public DistributionTypeEnum DistributionType
        {
            get
            {
                return _DistributionType;
            }
            protected set
            {
                _DistributionType = value;
            }
        }

        public RandomParameter()
        {
            Name = "RandomParameter"; //MPD: should we keep a count of these and automatically suffix new ones?
        }


    }


    public class RandomFlatParameter : RandomParameter
    {
        private double _UpperLimit;
        private double _LowerLimit;

        [Category("Limits")]
        public double UpperLimit
        {
            get
            {
                return _UpperLimit;
            }
            set
            {
                _UpperLimit = value;
            }
        }

        [Category("Limits")]
        public double LowerLimit
        {
            get
            {
                return _LowerLimit;
            }
            set
            {
                _LowerLimit = value;
            }
        }
        
        public RandomFlatParameter()
        {
            DistributionType = DistributionTypeEnum.Flat;
        }

        
    }


    public class RandomGaussParameter : RandomParameter
    {

        private double _Mean = 0.0;
        private double _StdDev = 0.0;

        [Category("Range")]
        public double Mean
        {
            get
            {
                return _Mean;
            }
            set
            {
                _Mean = value;
            }

        }
        
        [Category("Range")]
        public double StdDev
        {
            get
            {
                return _StdDev;
            }
            set
            {
                _StdDev = value;
            }
        }

        public RandomGaussParameter()
        {
            DistributionType = DistributionTypeEnum.Gaussian;
        }

    }

 

}