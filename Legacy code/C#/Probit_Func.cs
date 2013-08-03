
using System;
using System.IO;
using System.Text;


namespace PsychComponent
{
    

    internal class DataInput 
    {
        public int N_points; 
        public double[] StimLevelArray;// = new double[Max_Input - 1]; 
        public int[] ResponseArray; //= new int[Max_Input - 1]; 
        public int[] TotalObsArray ;//= new int[Max_Input - 1]; 
        public bool Fit50_100;
        public DataInput(int NoOfPoints)
        {
            N_points = NoOfPoints;
            // Initialise arrays
            StimLevelArray = new double[N_points];
            ResponseArray= new int[N_points]; 
            TotalObsArray= new int[N_points]; 
        }
    }

    internal class DataOutput 
    {
        public string OutPutString; 
        public double Threshold; 
        public double SE; 
        public double Chi; 
        public double ChiSqrCritical;
        public int N_points;
        public double[] ObservedArray;// = new double[Max_Input - 1]; 
        public double[] PredictArray;// = new double[Max_Input - 1]; 

        public DataOutput(int NoOfPoints)
        {
            N_points = NoOfPoints;
            ObservedArray = new double[NoOfPoints];
            PredictArray = new double[NoOfPoints];
        }
    }
		
    //  Equation of a straight line data structure
    internal struct EquLine 
    {
	         
	     public double gradient; 
	     public double intercept;
    }
		
		
	internal struct ThresholdOutput 
    {
	     public double Threshold; 
	     public double ThresholdError; 
	     public double lMean; 
	     public double MeanError; 
	     public double ChiStat; 
	     public double CriticalChi; 
    }
		
		
	internal class ChiOutput 
    {
        public double Likelihood; 
        public double ChiSqu; 
        public double[] PointChi;     
        public double[] ProbPredY; 
        public double lMean; 
        public double Threshold;
        public ChiOutput(int NoOfPoints)
        {
            PointChi = new double[NoOfPoints];
            ProbPredY = new double[NoOfPoints];
        }            
    }
		
		
	internal struct ProbitOutput 
    {
	     public EquLine Line;
	     public double SumXSq; 
	     public double MeanX; 
	     public double SumNObs; 
    }

	internal class Probit
	{

        public static readonly int Max_Input = 13;
        private static readonly int Maxpts = 13; //  not likely to have more than 13 points 
        private const double ThresholdConst =  0.67449;   //QU: AEW = magic number?
        private const double LikelihoodCriterion =  0.005; /* 76% correct is a d' (d prime) of 0.67449*/ /* p val for chi sqr*/
        /*function  CalcDesiredThreshold (DesiredThreshold, L_Intercept, L_Gradient, MinStim, MaxStim : double) : double;*/
        private int Num_DataPoints =  0; 
		
        //TODO: change these to dynamic arrays
        double[] StimulusValue; 
        int[] NumObservations; 
        double[] ObservedProportion; 
        double[] NormalizedProportion; 
		
        //double[] point_chisq; this seems to be unused
        private bool  Fit50to100 =  true; 
        //private string ThreshErrorString; 

		//  fit 50 to 100 or 0 to 100
		
		
		
		internal DataOutput DoProbit( DataInput DataIn)
		{
		 
            StringBuilder sb=new StringBuilder();
            string TempString; 
            bool Continue; 
            int i;
            DataOutput TempOutData = new DataOutput(DataIn.N_points);
            DataOutput Result=new DataOutput(DataIn.N_points);
		
		    Continue = true;
		    Num_DataPoints = DataIn.N_points;
		
            // set array sizes
            
            StimulusValue=new double[Num_DataPoints];
            NumObservations=new int[Num_DataPoints];
		
            if( DataIn.Fit50_100)
            {
                Fit50to100 = true;
                sb.AppendLine( "50 to 100 probit fit");
            }
            else
            {
                Fit50to100 = false;
                sb.AppendLine( "0 to 100 probit fit");
            }
		
		
		
            if( Num_DataPoints < 3 )
            {
                sb.AppendLine( "Too few good points to perform probit!");
                Continue = false;
            }

            ObservedProportion = new double[Num_DataPoints];
            for( i = 0;i < Num_DataPoints;i++)
            {
                StimulusValue[i] = DataIn.StimLevelArray[i];
                NumObservations[i] = DataIn.TotalObsArray[i];
                if( NumObservations[i] < 1 )
                {
                    sb.AppendLine( "Total number of observations must be > 0");
                    Continue = false;
                }

                ObservedProportion[i] =(double)((double) DataIn.ResponseArray[i])/(double)NumObservations[i];

                //  Check that obs. props. are in the range 0 < x < 1 (modify if nec.)
                if( ObservedProportion[i] < 0.00001 ) ObservedProportion[i] = 0.00001;
                if( ObservedProportion[i] > 0.99999 ) ObservedProportion[i] = 0.99999;
            }
		
		
		    if( Continue ) 
                TempOutData = RunProbit();
            
            TempString = sb.ToString();
		    TempOutData.OutPutString = TempOutData.OutPutString + TempString;
		    Result = TempOutData;
		    return Result;
		}
		
		
		
		
		internal DataOutput RunProbit()
		{
            const int Maxiter =  40; 
            // StimValueMedian : double;
            EquLine TempLineEquation=new EquLine();
            ChiOutput  TempChiValues=new ChiOutput(Num_DataPoints);
            ProbitOutput TempProbitOut=new ProbitOutput();
            ThresholdOutput TempThreshOut=new ThresholdOutput();
            double LineGrad,LineIntercept; 
            double Likelihood,ChiSqr,ChiMean,ChiThreshold; 
            double[] PointChi = new double[Num_DataPoints]; 
            double Old_intercept,
            Old_Gradient,
            Old_ChiSqr,
            Old_Likelihood; 
		
            //double DesiredLevel, MinVal, MaxVal; 
            //string DesiredThreshold; 
            int i, iterCount; 
            bool LSQFailed; 
            
            StringBuilder sb=new StringBuilder();

            DataOutput TempOut = new DataOutput(Num_DataPoints);
            DataOutput Result = new DataOutput(Num_DataPoints);


            NormalizedProportion = new double[Num_DataPoints];
            //  Calculate the Normal Equivalent Devitate for each observered proportion----
            for( i = 0;i < Num_DataPoints;i++)
                NormalizedProportion[i] = Calculate_NED(ObservedProportion[i]);
		
            //  Calculate values of gradient and intercept of equation relating Obs and Norm using LeastSquFit 
            TempLineEquation=LeastSquaresFit(Num_DataPoints, StimulusValue, NormalizedProportion); 
		
            if( TempLineEquation.gradient == -8888 )  //  Least squares fit failed
            {
                sb.AppendLine( "LSQ Fit Failed!");// +"\r""\n";
                LSQFailed = true;
            }
		    else 
                LSQFailed = false;
		
            if( ! LSQFailed )
            {
                LineGrad = TempLineEquation.gradient;
                LineIntercept = TempLineEquation.intercept;

                TempChiValues=Calc_GoodnessOfFit (Num_DataPoints, LineIntercept, LineGrad, NumObservations, StimulusValue, ObservedProportion);

                Likelihood = TempChiValues.Likelihood;
                ChiSqr = TempChiValues.ChiSqu;
                ChiMean = TempChiValues.lMean;
                ChiThreshold = TempChiValues.Threshold;

                iterCount = 1;
                bool doprobit=true;
                //continue = true;
		
                //  This is the iterative probit fit-------------------------------------------
                while( doprobit )
                {
                    iterCount++; // Keep track of how many iterations we do

                    Old_intercept = LineIntercept;  // Store the previous values
                    Old_Gradient = LineGrad;
                    Old_ChiSqr = ChiSqr;
                    Old_Likelihood = Likelihood;

                    TempProbitOut = ProbitCycle(Num_DataPoints, LineIntercept, LineGrad, NumObservations, StimulusValue, ObservedProportion);
                    LineGrad = TempProbitOut.Line.gradient;
                    LineIntercept = TempProbitOut.Line.intercept;

                    TempChiValues=Calc_GoodnessOfFit (Num_DataPoints, LineIntercept, LineGrad, NumObservations, StimulusValue, ObservedProportion);
                    Likelihood = TempChiValues.Likelihood;
                    ChiSqr = TempChiValues.ChiSqu;
                    ChiMean = TempChiValues.lMean;
                    ChiThreshold = TempChiValues.Threshold;

                    if( Likelihood < (Old_Likelihood-0.0001) )
                    {
                        if( iterCount > 3 )
                        {
                            sb.AppendLine( "Convergence is not good - likelihood didn't get better during iterative fit" );//+"\r""\n";
                            doprobit = false;
                            break;
                        }
                    }

                    if( iterCount > Maxiter )
                    {
                        sb.AppendLine( "Could not converge within " + Convert.ToString( Maxiter ) + " fit iterations" );//+"\r""\n";
                        doprobit = false;
                        break;
                    }

                    if( Likelihood < (Old_Likelihood + LikelihoodCriterion) )      // 0.05
                    {
                        if( Likelihood >= Old_Likelihood )
                        {
                            if( iterCount >1 )
                            {   
                                sb.AppendLine( "Normal Convergence");// +"\r""\n";
                                doprobit = false;
                                break;
                            }
                        }
                    }
                    if( Num_DataPoints < 3 )
                    {
                        sb.AppendLine( "Did not need iterations for a 2 point fit");// +"\r""\n";
                        doprobit = false;
                        break;
                    }
                } // while
		
		        TempThreshOut=EstimateError(Num_DataPoints, ChiSqr, LineIntercept, LineGrad, TempProbitOut.MeanX, TempProbitOut.SumXSq, TempProbitOut.SumNObs);
		
                for( i = 0;i <= Num_DataPoints-1;i++)
                {
                    TempOut.ObservedArray[i] = ObservedProportion[i];
                    TempOut.PredictArray[i] = TempChiValues.ProbPredY[i];
                }
		
                TempOut.Chi = ChiSqr;
                TempOut.ChiSqrCritical = TempThreshOut.CriticalChi;
                TempOut.Threshold = TempThreshOut.lMean;
                TempOut.SE = TempThreshOut.MeanError;
		
                if( TempThreshOut.ChiStat == 0)
                    sb.AppendLine( "Good Chi Fit");// +"\r""\n";
                else 
                   sb.AppendLine( "Chi Stat is too big!");// +"\r""\n";
		
                TempOut.OutPutString+=sb.ToString();
                Result = TempOut;
            } // not LSQfailed
		    return Result;
        }
		
		
		
		// //////////////////////////////////////////////////////////////////////////////
		//                                                                             //
		//             F U N C T I O N S   U S E D   I N   P R O B I T                 //
		//                                                                             //
		// //////////////////////////////////////////////////////////////////////////////
		
		
		// ------------------------------------------------------------------------------------------
		//   CALCULATE_NED                                                                         //
		//   This Function returns the Normal Equivalent Deviate of the observed proportion (p).   //
		//   0 < p < 1 is the range of values it can take.                                         //
		//   If p is outside this range an answer is returned as if p = 0 or p = 1.                //
		//   If p = 0 then Result = -5                                                             //
		//   If p = 1 then Result = 5                                                              //
		//   The result corresponds to the integral from infinity to NED of a                      //
		//         Gaussian with area and variance of 1.                                           //
		//   This is a rational approximation to the NED from Abramowitz and Stegun eqn. 26.2.23.  //
		//   NED Error < 4.5 e -4.                                                                 //
		// ------------------------------------------------------------------------------------------
		
        internal double Sqr(double val)
        {
            return Math.Pow(val,2);
        }

		internal double Calculate_NED( double proportion)
		{
            const double c0 =  2.515517; 
            const double c1 =  0.802853; 
            const double c2 =  0.010328; 
            const double d1 =  1.432788; 
            const double d2 =  0.189269; 
            const double d3 =  0.001308;  bool minus; 
            double ModProp, t, TempResult, num, denom; 
            double Result;
		
            if( proportion <= 0 ) Result = -5;
            if( proportion >= 1 ) Result = 5;
		
            if( proportion > 0.5 )
            {
                minus = false;
                ModProp = 1.0 - proportion;
            }
            else
            {
                minus = true;
                ModProp = proportion;
            }

            t = Math.Sqrt( Math.Log10(1/(Sqr(ModProp))) );

            denom = 1 + d1*t + d2*Sqr(t) + d3*Sqr(t)*t;
            num = c0 + c1*t + c2*Sqr(t);
            TempResult = t - num/denom;

            if( minus ) TempResult = (-1)*TempResult;
            if( TempResult < -5 ) TempResult = -5;
            if( TempResult > 5 ) TempResult = 5;
        
            Result = TempResult;
            return Result;
        }
		
		 // Calcualate_NED
		
		
		// -----------------------------------------------------------------------------------------------------------
		//   LEASTSQUARESFIT                                                                                        //
		//   This function calculates the least squares fit to the data.                                            //
		//                                                                                                          //
		//   The observed proportion correct (x) is plotted against the Normal Equivalent Deviate                   //
		//       obtained for that value of x (ie NED(x)).                                                          //
		//       Abcissa = NED(x)                                                                                   //
		//       Ordinate = x                                                                                       //
		//   The least squares method is then applied to ensure the smallest deviations from the                    //
		//   predicted value of x, from the value of x expected from the fitted line between the data.              //
		//                                                                                                          //
		//   i.e.                                                                                                   //
		//   Fit the line y = a + bx                                                                                //
		//   deviation = x (observed) - x^ (predicted).                                                             //
		//   The sum of these deviations should be as small as possible for the best fit.                           //
		//                                                                                                          //
		//   Once it has worked out the best fit, we are interested in the values of a and b from the fitted line.  //
		//                                                                                                          //
		//   b =  (Sum(xy)/n) - (Sum(x)*Sum(y)/n)                                                                   //
		//       ---------------------------------                                                                  //
		//        (Sum(x^2)/n) - (Sum(x)*Sum(x)/n)                                                                  //
		//                                                                                                          //
		//   a = mean(y)- b*mean(x)                                                                                 //
		//                                                                                                          //
		//   The output of this function is either a 1 (sucess) or 0 (failure)                                      //
		//   The values of a and b are also changed from their original values                                      //
		// -----------------------------------------------------------------------------------------------------------
		
        internal EquLine LeastSquaresFit( int NdataPoints, double[] StimVal,double[] Norm_Obs)
        {
            int Num_goodpoints; 
            int i; 
            double Sumx, Sumy,
            Sumxy, Sumx2,
            Meanx, Meany,
            Meanxy, Meanx2,
            num, denom;
            EquLine TempEquLine = new EquLine();

            double[] GoodStimulusValue = new double[NdataPoints]; 
            double[] GoodNormalized = new double[NdataPoints]; 
            
            if( NdataPoints < 2 )
            {
                // LeastSquaresCheckBox1.Checked := true;  // Show message that there is only one point
                TempEquLine.gradient = -8888;          //  Error value
            }

            Num_goodpoints = 0;
            for( i = 0;i <= NdataPoints-1;i++)
            if( (Norm_Obs[i] > -2.3) && (Norm_Obs[i] < 2.3) )
            {
                GoodStimulusValue[Num_goodpoints] = StimVal[i];
                GoodNormalized[Num_goodpoints] = Norm_Obs[i];
                Num_goodpoints++;
            }

            if( Num_goodpoints < 2 )
            {
                Num_goodpoints = NdataPoints;

                for( i = 0;i <= NdataPoints-1;i++) //TODO: AEW why -1 here?
                {
                    GoodStimulusValue[i] = StimVal[i];
                    GoodNormalized[i] = Norm_Obs[i];
                }
            }

            Sumx = 0;
            Sumy = 0;
            Sumxy = 0;
            Sumx2 = 0;

            if( Num_goodpoints != NdataPoints ) Num_goodpoints = Num_goodpoints -1; //TODO: AEW why do this?

            for( i = 0;i <= Num_goodpoints-1;i++)
            {
                Sumx = Sumx + GoodStimulusValue[i];
                Sumy = Sumy + GoodNormalized[i];
                Sumxy = Sumxy + GoodStimulusValue[i]*GoodNormalized[i];
                Sumx2 = Sumx2 + GoodStimulusValue[i]*GoodStimulusValue[i];
            }
            
            Meanx = Sumx / Num_goodpoints;
            Meany = Sumy / Num_goodpoints;
            Meanxy = Sumxy / Num_goodpoints;
            Meanx2 = Sumx2 / Num_goodpoints;

            num = Meanxy - Meanx*Meany;
            denom = Meanx2 - Meanx*Meanx;

            if( denom != 0)
            {
                TempEquLine.gradient = num / denom;
                TempEquLine.intercept = Meany - TempEquLine.gradient*Meanx;
            }
            else
            {
                // LeastSquaresCheckBox2.Checked := true; // Show message that there is effectively only one data point
                TempEquLine.gradient = -8888;         //  Error value
            }

            return TempEquLine;
        }
		
		 // Procedure TForm1.LeastSquaresFit
		
		
		// ------------------------------------------------------------------------
		//  CALC_GOODNESSOFFIT                                                   //
		//  This routine calculates chisq and like, the chi-squared and          //
		//  log likelihood of the current hypothesis: N[] = anew + bnew*x[]      //
		// ------------------------------------------------------------------------
        internal ChiOutput Calc_GoodnessOfFit( int NdataPoints, double L_intercept,double L_gradient, int[] Num_Obs, double[] StimVal,double[] Observed)
        {
            int i; 
            double PredictedY; 
            double Inverse_ProbPredY; 
            double Inverse_ObsY; 
            double Chi; 
            double[] ProbPredY = new double[NdataPoints];
            ChiOutput TempChiOutput = new ChiOutput(NdataPoints);

            Chi = 0;
            TempChiOutput.Likelihood = 0;
            TempChiOutput.ChiSqu = 0;

            for( i = 0;i <= NdataPoints-1;i++)
            {
                PredictedY = L_intercept + L_gradient * StimVal[i]; // Work out predicted value of y from our fitted line
                ProbPredY[i] = Calc_GaussianIntegral(PredictedY);  // Normalize to calculate the probability of the predicted value by working out the area under the gausian function

                if( ProbPredY[i] < 0.0001 ) ProbPredY[i] = 0.0001;  // ensure that the value is in range
                if( ProbPredY[i] > 0.9999 ) ProbPredY[i] = 0.9999;

                TempChiOutput.ProbPredY[i] = ProbPredY[i];
                Inverse_ProbPredY = 1.0 - ProbPredY[i];      // area on the other side of the predicted value of y
                Inverse_ObsY = 1.0 - Observed[i];  // area on the other side of the observed value of y

                //  Now work out the likelihood...
                //  Calculate the product of predicted and observed weighted by the number of observations,
                //  added to the the inverse proportion of the predicted and observed values.

                TempChiOutput.Likelihood = TempChiOutput.Likelihood + Num_Obs[i] * (Observed[i]*Math.Log10(ProbPredY[i])+Inverse_ObsY*Math.Log10(Inverse_ProbPredY));

                //  Now work out the Chi square value for each point
                //  Chi square = weighting factor * (observed - predicted proportion)^2
                //               ------------------------------------------------------
                //                       Predicted proportion * (1-predicted)

                Chi = Num_Obs[i]*(Sqr(Observed[i]-ProbPredY[i]))/(ProbPredY[i]*Inverse_ProbPredY);
                TempChiOutput.PointChi[i] = Chi;  // record the individual chi value
                TempChiOutput.ChiSqu = TempChiOutput.ChiSqu + Chi;  //  This is the sum of all the individual chi square values
            }

            if( System.Math.Abs(L_gradient) > 0.00001)
            {
                TempChiOutput.lMean = -L_intercept/L_gradient;              // *************************** I don't understand this bit ******************************** //
                TempChiOutput.Threshold = ThresholdConst/L_gradient;
            }
            else
            {  //  Can't obtain a mean or a threshold: so throw out something stupid
                TempChiOutput.lMean = -L_intercept * 100000.0;
                TempChiOutput.Threshold = 100000.0;
            }
            
           return TempChiOutput;
        }
		
		
		
		
		// -----------------------------------------------------------------------------
		//   CALC_GAUSSIANINTEGRAL                                                    //
		//   This routine calculates the Gaussian Integral from - infinity to z;      //
		//   that is, the integral of exp(-t*t)/sqrt(2*pi). ie probability estimate   //
		//                                                                            //
		//   gaussint(z) = (1 + erf(z/sqrt(2)) ) / 2                                  //
		//                                                                            //
		//   erf is the error function.                                               //
		//   This rational approx. to erf is from Abramowitz and Stegun eqn. 7.1.26.  //
		//   erf error < 1.5 e -7.                                                    //
		// -----------------------------------------------------------------------------
		
        internal double Calc_GaussianIntegral( double z)
        {
            const double p =    0.3275911; 
            const double a1 =    0.254829592; 
            const double a2 =   -0.284496736; 
            const double a3 =   1.421413741; 
            const double a4 =  -1.453152027; 
            const double a5 =   1.061405429;  double x, t, erf, g, sign; 
            double Result;

            if( z < 0 )
            {
                sign = -1.0;
                z= -z;
            }
            else sign = 1.0;

            x = z / Math.Sqrt(2);
            t = 1.0 / (1.0 + p*x);
            erf = a1*t + a2*Sqr(t) + a3*Sqr(t)*t + a4*Sqr(t)*Sqr(t) + a5*Sqr(t)*Sqr(t)*t;
            erf = sign * ( 1.0 - erf*Math.Exp(-x*x) );
            g = (1.0 + erf) / 2.0;

            if( Fit50to100 ) g = (1+g)/2;  //  if fit is 50% TO 100%

            Result = g;
            return Result;
        }
		
		
		
		// -----------------------------------------------------------------------------
		//  PROBITCYCLE                                                               //
		//  This routine calculates one cycle of Finney's probit algorithm.           //
		//                                                                            //
		// -----------------------------------------------------------------------------
		
		internal ProbitOutput ProbitCycle( int NdataPoints, double L_intercept,double L_gradient, int[] Num_Obs, double[] StimVal,double[] Observed)
        {
            int i; 
            double Gaus_val; 
            double PredictedY; 
            double Inverse_ProbPredY; 
            double[] ProbPredY = new double[Maxpts]; 
            double[] Weights = new double[Maxpts]; 
            double[] Probits = new double[Maxpts]; 
            double Sumx, Sumy,
            Sumx2, Sumy2,
            Sumxy, SumWObs,
            meanx, meany;
            EquLine TempEquLine = new EquLine();
            ProbitOutput TempOutput = new ProbitOutput();


            Sumx = 0;
            Sumy = 0;
            Sumx2 = 0;
            Sumy2 = 0;
            Sumxy = 0;
            SumWObs = 0;

            for( i = 0;i <= NdataPoints-1;i++)
            {
                PredictedY = L_intercept + L_gradient * StimVal[i]; // Work out predicted value of y (NED) from the best-fit line

                Gaus_val = Math.Exp(-Sqr(PredictedY)/2) /(Math.Sqrt(2*Math.PI));  //  Value of Gaussian at point Y
                if( Gaus_val < 0.0001 ) Gaus_val = 0.0001;       //  prevents explosion for small Gaus values

                ProbPredY[i] = Calc_GaussianIntegral(PredictedY);  // Calculate the probability of the predicted value by working out the area under the gausian function

                if( ProbPredY[i] < 0.0001 ) ProbPredY[i] = 0.0001;  // ensure that the value is in range
                if( ProbPredY[i] > 0.9999 ) ProbPredY[i] = 0.9999;

                Inverse_ProbPredY = 1.0 - ProbPredY[i];

                Weights[i] = Gaus_val*Gaus_val / (ProbPredY[i]*Inverse_ProbPredY); //  weights points using some form of likelihood estimate
                Probits[i] = PredictedY + (Observed[i]-ProbPredY[i])/Gaus_val; //  Working Probits

                Sumx = Sumx + Num_Obs[i] * Weights[i] * StimVal[i];
                Sumy = Sumy + Num_Obs[i] * Weights[i] * Probits[i];
                SumWObs = SumWObs + Num_Obs[i] * Weights[i];       //  Sum of the # of observations * weights
            }

            // Make sure Meanx and Meany have a value
            meanx = StimVal[1];
            meany = Probits[1];

            if( SumWObs != 0 )
            {
                meanx = Sumx/SumWObs;
                meany = Sumy/SumWObs;
            }

            for( i = 0;i <= NdataPoints-1;i++)
            {
                Sumx2 = Sumx2 + Num_Obs[i] *Weights[i] *(StimVal[i]-meanx)*(StimVal[i]-meanx);
                Sumy2 = Sumy2 + Num_Obs[i] *Weights[i] *(Probits[i]-meany)*(Probits[i]-meany);
                Sumxy = Sumxy + Num_Obs[i] *Weights[i] *(StimVal[i]-meanx)*(Probits[i]-meany);
            }

            if( NdataPoints <3)
            {
                TempEquLine.gradient = L_gradient;
                TempEquLine.intercept = L_intercept;
            }
            else
            {
                if( Sumx2 > 0.00001)
                    TempEquLine.gradient = Sumxy/Sumx2;
                else 
                    TempEquLine.gradient = -8888;

                TempEquLine.intercept = meany - TempEquLine.gradient*meanx;
            }

            TempOutput.Line = TempEquLine;
            TempOutput.SumXSq = Sumx2;
            TempOutput.MeanX = meanx;
            TempOutput.SumNObs = SumWObs;

            return TempOutput;
        }
		
		
		
		// -----------------------------------------------------------------------------
		//   ESTIMATEERROR                                                            //
		//   This routine estimates the error in the mean and threshold.              //
		//   The formulae give one-sigma errors, and are correct if the probit        //
		//   fit is good.  If chisq is too big, the heterogeneity factor is           //
		//   calculated, but is not used in any way.  Suzanne talked me out of        //
		//   simply inflating the errors like Finney; if chisq is too high, you       //
		//   make your own estimate of what to do about it.  In general, it means     //
		//   thet your data are not describable as a cumulative normal.               //
		// -----------------------------------------------------------------------------
		
		internal ThresholdOutput EstimateError( int NdataPoints, double ChiSqr,double L_intercept,double L_gradient,double MeanX,double SumxSq,double SumNObs)
		{
            const int t =  1;  /* One sigma-errors*/; 
            double Thresh, ThreshErr,lMean, MeanErr,ChiStat,temperr,g, dof, c10;
            ThresholdOutput TempThreshOutput = new ThresholdOutput();

            if( System.Math.Abs(L_gradient) < 0.0001)

            {
                lMean = -8888;
                Thresh = -8888;
                MeanErr = -8888;
                ThreshErr = -8888;
                ChiStat = 10.0;
            }
            else
            {
                if( NdataPoints> 2)
                {
                    dof = NdataPoints -2;
                    g = Math.Sqrt(2*dof -1);
                    c10= 0.65 + 1.3*g + Sqr(g)/2;  // c10 is the area under the chi distribution at the 0.1 sig level for the appropriate df
                    TempThreshOutput.CriticalChi = c10;

                    if( ChiSqr < c10) 
                        ChiStat = 0.0          /*ChiSq is okay*/;
                    else 
                        ChiStat = ChiSqr/dof;   // ChiSq is too big
                }
                else ChiStat = 0.0;

                lMean = -L_intercept/L_gradient;    // ///888888888888888888888  don't understand 8888888888888888888///
                Thresh= ThresholdConst/L_gradient;

                if( SumxSq == 0 ) SumxSq = 0.0001;
                if( SumNObs == 0 ) SumNObs = 0.0001;

                g = Sqr(t)/(Sqr(L_gradient)*SumxSq);
                if( g == 0 ) g = 0.0001;
                if( g == 1 ) g = 0.9999;

                temperr = ((1-g)/SumNObs) + (Sqr((lMean-MeanX))/SumxSq);

                MeanErr = System.Math.Abs(t/L_gradient/(1-g)) *Math.Sqrt(temperr);
                ThreshErr = (Thresh*Math.Sqrt(g))/(1-g);
            }

            TempThreshOutput.lMean = lMean;
            TempThreshOutput.MeanError = MeanErr;
            TempThreshOutput.Threshold = Thresh;
            TempThreshOutput.ThresholdError = ThreshErr;
            TempThreshOutput.ChiStat = ChiStat;

            return TempThreshOutput;
        }

// EstimateError

/*        internal string RToStr( double realnum)
        {
            string TempStr; 
            string Result;

            TempStr = FloatToStrF(realnum, ffFixed, 6, 4) ;
            Result = TempStr;
            return Result;
        }

*/
	}
}
 