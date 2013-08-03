using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PsychComponent
{
    public class Staircase 

    {
        //MPD: serialization - need to implement serialization for list params etc. for component or we serialize the whole class into code?

        private uint _MaxReversals; // 
        private StaircaseTypeEnum _StaircaseType;
        private StepSizeTypeEnum _StepSizeType;
        private float _MaxStimVal;
        private float _MinStimVal;
        private float _ScaleFactorPercentage;
        private int _MaxTrials;
        private uint _MaxBoundaries;
        private bool _RandomizeStimVal;
        private float _StimValRandPercentage;
        private List<float> _FixedStepSizes;
        protected int _STCDown; 
        protected int _STCUp; 
        private uint _IgnoreReversals;
        private double _InitialStimVal;
        private int _Index;
        
        public Staircase()
        {
            _MaxReversals=12;
            _MaxStimVal=1;
            _MinStimVal=-1;
            _ScaleFactorPercentage = 3;
            _MaxTrials=100;
            _RandomizeStimVal=false;
            _StimValRandPercentage=5;
            _InitialStimVal = 0;
            _MaxBoundaries = 3;
            StaircaseType = StaircaseTypeEnum.st1up_2down;
            _StepSizeType = StepSizeTypeEnum.FixedStepSize;
            _FixedStepSizes = new List<float>();
            _Index=-1; // uninitialised until we use it to form runtime staircases
            _IgnoreReversals = 4;

        }

        public Staircase(Staircase staircase)
        {
            _MaxReversals = staircase.MaxReversals;
            _MaxStimVal = staircase.MaxStimVal;
            _InitialStimVal = staircase.InitialStimVal;
            _FixedStepSizes = staircase.FixedStepSizes;
            _IgnoreReversals = staircase.IgnoreReversals;
            _MaxBoundaries = staircase.MaxBoundaries;
            _MaxTrials = staircase.MaxTrials;
            _MinStimVal = staircase.MinStimVal;
            _RandomizeStimVal = staircase.RandomizeStimVal;
            _ScaleFactorPercentage = staircase.ScaleFactorPercentage;
            StaircaseType = staircase.StaircaseType;
            _STCDown = staircase.STCDown;
            _STCUp = staircase.STCUp;
            _StepSizeType = staircase.StepSizeType;
            _StimValRandPercentage = staircase.StimValRandPercentage;
            _Index = -1;
        }

        [Browsable(false)]
        internal int Index
        {
            get
            {
                return _Index;
            }

            set
            {
                _Index = value;
            }
        }

        public bool Validate()
        {
            //TODO: Implement staircase validation
            return true;
        }

        public double InitialStimVal
        {
            get
            {
                return _InitialStimVal;
            }
            set
            {
                _InitialStimVal = value;
            }
        }

        [Category("Counters")]
        public uint UsefulReversals
        {
            get
            {
                return _MaxReversals-_IgnoreReversals;
            }
        }

        public uint IgnoreReversals
        {
            get
            {
                return _IgnoreReversals;
            }
            set
            {
                _IgnoreReversals = value;
            }
        }

        public uint MaxReversals
        {
            get
            {
                return _MaxReversals;
            }
            set
            {
                _MaxReversals=value;
            }
        }

        public uint MaxBoundaries
        {
            get
            {
                return _MaxBoundaries;
            }
            set
            {
                _MaxBoundaries = value;
            }
        }


        public StaircaseTypeEnum StaircaseType
        {
            get
            {
                return _StaircaseType;
            }
            set
            {
                _StaircaseType=value;
                int scval =(int) Enum.Parse(typeof(StaircaseTypeEnum), Enum.GetName(typeof(StaircaseTypeEnum), value));
                int up, down;
                down = scval % 10;
                up = (scval - down) / 10;

                _STCDown = down;
                _STCUp = up;

            }
        }

        public StepSizeTypeEnum StepSizeType
        {
            get
            {
                return _StepSizeType;
            }
            set
            {
                _StepSizeType=value;
            }
        }

        [Category("Stimulus")]
        public float MaxStimVal
        {
            get
            {
                return _MaxStimVal;
            }
            set
            {
                _MaxStimVal=value;
            }
        }

        [Category("Stimulus")]
        public float MinStimVal
        {
            get
            {
                return _MinStimVal;
            }
            set
            {
                _MinStimVal=value;
            }
        }

        public float ScaleFactorPercentage
        {
            get
            {
                return _ScaleFactorPercentage; 
            }
            set
            {
                _ScaleFactorPercentage=value;
            }
        }

        public int MaxTrials
        {
            get
            {
                return _MaxTrials;
            }
            set
            {
                _MaxTrials=value;
            }
        }

        public bool RandomizeStimVal
        {
            get
            {
                return _RandomizeStimVal;
            }
            set
            {
                _RandomizeStimVal=value;
            }
        }

        public float StimValRandPercentage
        {
            get
            {
                return _StimValRandPercentage;
            }
            set
            {
                _StimValRandPercentage = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<float> FixedStepSizes
        {
            get
            {
                return _FixedStepSizes;
            }
            protected set
            {
                _FixedStepSizes = value;
            }
            
        }

        [Category("Info")]
        public int STCUp
        {
            get
            {
                return _STCUp;
            }
        }

        [Category("Info")]
        public int STCDown
        {
            get
            {
                return _STCDown;
            }
            
        }
    }

    public class RuntimeStaircase : Staircase
    {
        private int _RightSinceWrong;
        private int _WrongSinceRight;
        private int _ReversalCount;
        private int _OutOfBoundariesCount;
        private int _TrialCount;
        private double _StimVal;
        private int _Condition;
        private List<double> _ReversalValues;
        private StepTypeEnum _StepType;
        private StaircaseStatusEnum _Status;
        private int _StaircaseIndex=-1;
        private int _RuntimeIndex=-1;
        private List<int> _ReversalTrials;

        private RuntimeStaircase()
        {

        }

        public RuntimeStaircase(Staircase staircase, int condition)
        {
            // construct staircase from default stair case parameters
            
            _RightSinceWrong = 0;
            _WrongSinceRight = 0;
            _ReversalCount = 0;
            _OutOfBoundariesCount=0;
            _TrialCount = 0;
            _Condition = condition;
            _ReversalValues= new List<double>();
            _ReversalTrials = new List<int>();
            _Status = StaircaseStatusEnum.NotStarted;
            _StepType = StepTypeEnum.Start;
            

            this.FixedStepSizes = staircase.FixedStepSizes;
            this.MaxReversals = staircase.MaxReversals;
            this.StepSizeType = staircase.StepSizeType;
            this.StaircaseType = staircase.StaircaseType;
            this.ScaleFactorPercentage = staircase.ScaleFactorPercentage;
            this.MaxStimVal = staircase.MaxStimVal;
            this.MinStimVal = staircase.MinStimVal;
            this.MaxTrials = staircase.MaxTrials;
            this.StimValRandPercentage = staircase.StimValRandPercentage;
            this.RandomizeStimVal = staircase.RandomizeStimVal;
            this.FixedStepSizes = staircase.FixedStepSizes; // by ref copy
            this._STCUp = staircase.STCUp;
            this._STCDown = staircase.STCDown;
            this.StimVal = staircase.InitialStimVal;
            this.InitialStimVal = this.StimVal;
            this._StaircaseIndex = staircase.Index;
            this.Index = staircase.Index;
           
        }

        public List<int> ReversalTrials
        {
            get
            {
                return _ReversalTrials;
            }
        }

        public int RuntimeIndex
        {
            get
            {
                return _RuntimeIndex;
            }
            set
            {
                _RuntimeIndex = value;
            }
        }

        public int StaircaseIndex
        {
            get
            {
                return _StaircaseIndex;
            }
        }

        public int TrialCount
        {
            get
            {
                return _TrialCount;
            }
            set
            {
                _TrialCount = value;
            }
        }

        public int OutOfBoundariesCount
        {
            get
            {
                return _OutOfBoundariesCount;
            }
            set
            {
                _OutOfBoundariesCount = value;
            }
        }

        public int RightSinceWrong
        {
            get
            {
                return _RightSinceWrong;
            }
            set
            {
                _RightSinceWrong = value;
            }
        }

        public int WrongSinceRight
        {
            get
            {
                return _WrongSinceRight;
            }
            set
            {
                _WrongSinceRight = value;
            }
        }

        public int ReversalCount
        {
            get
            {
                return _ReversalCount;
            }
            set
            {
                _ReversalCount = value;
            }
        }

        public double StimVal
        {
            get
            {
                return _StimVal;
            }
            set
            {
                _StimVal = value;
            }
        }

        public int Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                Condition = value;
            }
        }

        public List<double> ReversalValues
        {
            get
            {
                return _ReversalValues;
            }
        }

        public StepTypeEnum StepType
        {
            get
            {
                return _StepType;
            }
            set
            {
                _StepType = value;
            }
        }

        public StaircaseStatusEnum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        
        
    }

}
