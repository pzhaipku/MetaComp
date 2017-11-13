using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using System.Data.OleDb;
using System.Data;

namespace MetaComp
{
    public class app
    {
        public static double[][] R;

        public static double[][] InverseMatrix(double[][] matrix)
        {
            if (matrix == null || matrix.Length == 0)
            {
                return new double[][] { };
            }


            int len = matrix.Length;
            for (int counter = 0; counter < matrix.Length; counter++)
            {
                if (matrix[counter].Length != len)
                {
                    throw new Exception("matrix must be square!");
                }
            }

            double dDeterminant = Determinant(matrix);
            if (Math.Abs(dDeterminant) <= 1E-6)
            {
                throw new Exception("Degenerate matrix!");
            }

 
            double[][] result = AdjointMatrix(matrix);

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    result[i][j] = result[i][j] / dDeterminant;
                }
            }

            return result;
        }

        public static double Determinant(double[][] matrix)
        {

            if (matrix.Length == 0) return 0;
            else if (matrix.Length == 1) return matrix[0][0];
            else if (matrix.Length == 2)
            {
                return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
            }


            double dSum = 0, dSign = 1;
            for (int i = 0; i < matrix.Length; i++)
            {
                double[][] matrixTemp = new double[matrix.Length - 1][];
                for (int count = 0; count < matrix.Length - 1; count++)
                {
                    matrixTemp[count] = new double[matrix.Length - 1];
                }

                for (int j = 0; j < matrixTemp.Length; j++)
                {
                    for (int k = 0; k < matrixTemp.Length; k++)
                    {
                        matrixTemp[j][k] = matrix[j + 1][k >= i ? k + 1 : k];
                    }
                }

                dSum += (matrix[0][i] * dSign * Determinant(matrixTemp));
                dSign = dSign * -1;
            }

            return dSum;
        }

        public static double[][] AdjointMatrix(double[][] matrix)
        {

            double[][] result = new double[matrix.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix[i].Length];
            }


            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result.Length; j++)
                {

                    double[][] temp = new double[result.Length - 1][];
                    for (int k = 0; k < result.Length - 1; k++)
                    {
                        temp[k] = new double[result[k].Length - 1];
                    }


                    for (int x = 0; x < temp.Length; x++)
                    {
                        for (int y = 0; y < temp.Length; y++)
                        {
                            temp[x][y] = matrix[x < i ? x : x + 1][y < j ? y : y + 1];
                        }
                    }

                    result[j][i] = ((i + j) % 2 == 0 ? 1 : -1) * Determinant(temp);
                }
            }

            return result;
        }
        public static double[][] MultipleMatrix(double[][] matrix1,double[][] matrix2)
        {
            double[][] result = new double[matrix1.GetLength(0)][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix2[0].GetLength(0)];
            }
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2[0].GetLength(0); j++)
                {
                    result[i][j] = 0;
                    for (int k = 0; k < matrix1[0].GetLength(0); k++)
                    {                        
                        result[i][j] = result[i][j]+matrix1[i][k]*matrix2[k][j];
                    }
                }
            }
            return result;
        }

        public static double[][] TMatrix(double[][] matrix)
        {
            double[][] result = new double[matrix[0].GetLength(0)][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix.GetLength(0)];
            }
            for (int i = 0; i < matrix[0].GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    result[i][j] = matrix[j][i];
                }
            }
            return result;
        }
        public static double[][] AddMatrix(double[][] matrix1,double[][]matrix2)
        {
            double[][] result = new double[matrix1.GetLength(0)][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix1[0].GetLength(0)];
                for (int j = 0; j < matrix1[0].GetLength(0); j++)
                {
                    result[i][j] = matrix1[i][j] + matrix2[i][j];
                }
            }
            return result;
        }
        public static double[][] MinusMatrix(double[][] matrix1, double[][] matrix2)
        {
            double[][] result = new double[matrix1.GetLength(0)][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix1[0].GetLength(0)];
                for (int j = 0; j < matrix1[0].GetLength(0); j++)
                {
                    result[i][j] = matrix1[i][j] - matrix2[i][j];
                }
            }
            return result;
        }
        public static int Add_Factors(double[][] Factor, double[][] Y, float alpha, double[][] R, List<int> id)
        {
            if (id.Count == Factor[0].GetLength(0))
                return 0;

            int l = id.Count;
            int n = Factor.GetLength(0);

            double Fvalue = app.Fvalue[n - l - 2];


            List<int> excludeid = new List<int>();
            for (int i = 1; i <= Factor[0].GetLength(0); i++)
            {
                excludeid.Add(i);
            }
            for (int i = 0; i < id.Count; i++)
            {
                int j = 0;
                while (j < excludeid.Count)
                { 
                    if (excludeid[j] == id[i])
                        excludeid.RemoveAt(j);
                    j++;
                }
            }
            double[] u = new double[excludeid.Count];
            for (int i = 0; i < u.GetLength(0); i++)
                u[i] = Math.Pow(R[excludeid[i] - 1][R[0].GetLength(0) - 1], 2) / R[excludeid[i] - 1][excludeid[i] - 1];
            int maxid = excludeid[0];
            int temp = 0;
            double maxu = u[0];
            for (int i = 0; i < excludeid.Count; i++)
            {
                if (u[i] > maxu)
                {
                    maxu = u[i];
                    maxid = excludeid[i];
                    temp = i;
                }
            }

            double U = R[R.GetLength(0) - 1][R[0].GetLength(0) - 1] - maxu;
            double F = maxu / (U / (n - l - 2));
            if (F > Fvalue)
                return maxid;
            else
                return 0;
        }

        
        public static int Delete_Factors(double[][] Factor, double[][] Y, float alpha, double[][] R, List<int> id)
        {
            if (id.Count == 1)
                return 0;

            int l = id.Count;
            int n = Factor.GetLength(0);

            double Fvalue = app.Fvalue[n - l - 1];

            double[] u = new double[id.Count - 1];
            for (int i = 0; i < u.GetLength(0); i++)
                u[i] = Math.Pow(R[id[i] - 1][R[0].GetLength(0) - 1], 2) / R[id[i] - 1][id[i] - 1];
            int minid = id[0];
            double minu = u[0];
            for (int i = 0; i < u.GetLength(0); i++)
            {
                if (u[i] < minu)
                {
                    minu = u[i];
                    minid = id[i];
                }
            }

            double F = minu / (R[R.GetLength(0) - 1][R[0].GetLength(0) - 1] / (n - l - 1));
            if (F <= Fvalue)
                return minid;
            else
                return 0;
        }

        public static int Check_Factors(double[][] Factor, double[][] Y, float alpha, double[][] R, List<int> id)
        {
            if (id.Count == 1)
                return 0;

            int l = id.Count;
            int n = Factor.GetLength(0);
            
            double Fvalue = app.Fvalue[n - l - 1];

            double[] u = new double[id.Count];
            for (int i = 0; i < u.GetLength(0); i++)
                u[i] = Math.Pow(R[id[i] - 1][R[0].GetLength(0) - 1], 2) / R[id[i] - 1][id[i] - 1];
            int minid = id[0];
            double minu = u[0];
            for (int i = 0; i < u.GetLength(0); i++)
            {
                if (u[i] < minu)
                {
                    minu = u[i];
                    minid = id[i];
                }
            }

            double F = minu / (R[R.GetLength(0) - 1][R[0].GetLength(0) - 1] / (n - l - 1));

            if (F <= Fvalue)
                return minid;
            else
                return 0;
        }

        public static double MEAN(double[] Num)
        {
            double mean = 0;
            for (int i = 0; i < Num.GetLength(0); i++)
            {
                mean = mean + Num[i];
            }
            mean = mean / Num.GetLength(0);
            return mean;
        }

        public static double sd(double[] Num)
        {
            double sd = 0;
            for (int i = 0; i < Num.GetLength(0); i++)
            {
                sd = sd + (Num[i] - app.MEAN(Num)) * (Num[i] - app.MEAN(Num));
            }
            sd = sd / (Num.GetLength(0) - 1);
            sd = Math.Sqrt(sd);
            return sd;
        }
        public static double[][] Cormatrix(double[][] matrix)
        { 
            

            double[][] result = new double[matrix[0].GetLength(0)][];
            for (int i = 0; i < result.GetLength(0); i++)
            { 
                result[i] = new double[matrix[0].GetLength(0)];
                for (int j = 0; j < result[i].GetLength(0); j++)
                { 
                    double[] X = new double[matrix.GetLength(0)];
                    double[] Y = new double[matrix.GetLength(0)];
                    double[] XY = new double[matrix.GetLength(0)];
                    double[] XX = new double[matrix.GetLength(0)];
                    double[] YY = new double[matrix.GetLength(0)];
                    for (int k = 0; k < matrix.GetLength(0); k++)
                    {
                        X[k] = matrix[k][i];
                        Y[k] = matrix[k][j];
                        XX[k] =  matrix[k][i] * matrix[k][i];
                        YY[k] =  matrix[k][j] * matrix[k][j];
                        XY[k] =  matrix[k][i] * matrix[k][j];
                    }
                    result[i][j] = ( app.MEAN(XY) - app.MEAN(X) * app.MEAN(Y) ) /  Math.Sqrt( ( app.MEAN(XX) - app.MEAN(X) * app.MEAN(X) ) * ( app.MEAN(YY) - app.MEAN(Y) * app.MEAN(Y) ) );
                }
            }
            return result;
        }

        public static double[][] Rconvert(double[][] Rmatrix, int mainindex)
        {
            double[][] result = new double[Rmatrix[0].GetLength(0)][];
            for(int i = 0; i < result.GetLength(0); i++)
            {
                result[i] = new double[Rmatrix[i].GetLength(0)];
            }
            int k = mainindex - 1;
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result[i].GetLength(0); j++)
                {
                    if ((i != k) && (j != k))
                        result[i][j] = Rmatrix[i][j] - Rmatrix[i][k] * Rmatrix[k][j] / Rmatrix[k][k];
                }
            }
            for (int j = 0; j < result[k].GetLength(0); j++)
            {
                if(j != k)
                    result[k][j] = Rmatrix[k][j] / Rmatrix[k][k];
            }
            for (int i = 0; i < result.GetLength(0); i++)
            {
                if (i != k)
                    result[i][k] = -Rmatrix[i][k] / Rmatrix[k][k];
            }
            result[k][k] = 1 / Rmatrix[k][k];
            return result;
        }
        public static float alphaen;
        public static string value;
        public static string[] FeaName;
        public static List<string> FeatureName;
        public static string[] FactorName;
        public static List<string> FinalFactorName;
        public static string[] SamName;
        public static List<string> SampleName;
        public static object Data;
        public static List<List<string>> WData;
        public static object FactorData;
        public static List<List<double>> Count;
        public static List<List<double>> FactorCount;
        public static List<List<double>> FinalFactorCount;
        public static List<List<double>> Freq;

        public static double[,] EFMatrix;
        public static double[,] CountMatrix;
        public static double[,] FreqMatrix;
        public static double[] SampleTotal;
        
        public static int[] cluster;
        public static int clusterNum;
        public static double[,] Score;
        public static double[] P;
        public static double[] R_square;
        public static double[][] T;
        public static double[][][] B;
        public static float alpha;
        public static List<List<int>> Allid;
        public static List<List<double>> pvalue;
        public static List<List<double>> Rvalue;
        public static List<double> Fvalue;
        public static DataSet OleDsExcleCOG;
        public static DataSet OleDsExclePFAM;
        public static bool EFAcheck;
        public static bool EFAradio1;
        public static bool EFAradio2;

        public double[,] inPut;
        public int k;
        public static double[,] Center;
        public static int[,] ClusterResult;

        public static DataTable EFProfile;
        public static DataTable Profile;
        public static DataTable COGdatabase;
        public static DataTable KEGGdatabase;

        public static double[,] GetProcess(double[,] input, int k)
        {
            int Num;
            int sub;
            int[] groupNum;
            Num = input.GetLength(1);
            sub = input.GetLength(0);
            groupNum = new int[k];
            double[,] tmpCenter = new double[sub, k];
            for (int i = 0; i < sub; i++)
                for (int j = 0; j < k; j++)
                    tmpCenter[i, j] = input[i, j];
            double[,] preCenter = new double[sub, k];
            double[,] resultP ;
            while (true)
            {
                 resultP = new double[k, Num + sub];

                #region //Clear
                for (int i = 0; i < k; i++)
                {
                    groupNum[i] = 0;
                }
                #endregion

                #region //Cluster

                for (int i = 0; i < Num; i++)
                {
                    double tmpDis = 0.0;
                    int index = 0;
                    for (int j = 0; j < k; j++)
                    {
                        double tmpIn = 0.0;
                        for (int m = 0; m < sub; m++)
                        {
                            tmpIn += Math.Pow((input[m, i] - tmpCenter[m, j]), 2);
                        }
                        if (j == 0)
                        {
                            tmpDis = tmpIn;
                            index = 0;
                        }
                        else
                        {
                            if (tmpDis > tmpIn)
                            {
                                tmpDis = tmpIn;
                                index = j;
                            }
                        }
                    }
                    int groupKnum = groupNum[index];
                    resultP[index, groupKnum] = i+1;
                    groupNum[index]++;
                }
                #endregion

                #region //Center
                for (int i = 0; i < sub; i++)
                    for (int j = 0; j < k; j++)
                        preCenter[i, j] = tmpCenter[i, j];
                #endregion

                #region //New Center
                for (int i = 0; i < k; i++)
                {
                    int kNum=groupNum[i];
                    if (kNum > 0)
                    {
                        for (int j = 0; j < k; j++)
                        {
                            double tmp = 0.0;
                            for (int m = 0; m < kNum; m++)
                            {
                                int groupIndex = (int)resultP[i,j]-1;
                                tmp += input[groupIndex, j];
                            }
                            tmpCenter[i, j] = tmp / kNum;
                        }
                    }
                }
                #endregion

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < sub; j++)
                        resultP[i, j + Num] = tmpCenter[j, i];
                }

                #region //Center change
                bool judge = true;
                for (int i = 0; i < sub; i++)
                {
                    for (int j = 0; j < k; j++)
                    {
                        judge = judge && (preCenter[i,j]==tmpCenter[i,j]);
                    }
                }
                if (judge)
                {
                    break;
                }
                #endregion
                }
            return resultP;
        }

        public static double[,] Standard(double[,] Input)
        {
        	int FeatureNum = Input.GetLength(0);
            int SampleNum = Input.GetLength(1);
          
            double[,] StandardMatrix = new double[FeatureNum,SampleNum];
            for(int i = 0;i < FeatureNum;i++)
            {
            	double Sum = 0;
            	for(int j = 0;j < SampleNum;j++)
            		Sum+= Input[i,j];
            	double mean = Sum / SampleNum;
            	double sigma = 0;
            	for(int j = 0;j < SampleNum;j++)
            		sigma+= Math.Pow(Input[i,j] - mean,2);
            	sigma = Math.Sqrt( sigma / (SampleNum - 1));
                for(int j = 0;j < SampleNum;j++)
            	    StandardMatrix[i,j] = (Input[i,j] - mean) / sigma;	
            }
            return StandardMatrix;
        }

        public static double[,] CoMatrix(double[,] Input)
        {
        	int FeatureNum = Input.GetLength(0);
            int SampleNum = Input.GetLength(1);
            
            double[,] NorStandardMatrix = new double[FeatureNum,SampleNum];
            double[] mean = new double[FeatureNum];
            double[,] CorMatrix = new double[FeatureNum,FeatureNum];
            for(int i = 0;i < FeatureNum;i++)
            {
            	double Sum = 0;
            	for(int j = 0;j < SampleNum;j++)
            		Sum+= Input[i,j];
            	mean[i] = Sum / SampleNum; 	
            }
            for(int i = 0;i < FeatureNum;i++)
            {
            	for(int j = 0;j < SampleNum;j++)
            	{
            		for(int l =0;l < FeatureNum;l++)
            		{
            			NorStandardMatrix[i,j]+= (Input[i,l] - mean[i]) * (Input[j,l] - mean[j]);
            		}
            		NorStandardMatrix[i,j] = NorStandardMatrix[i,j] / (SampleNum - 1);
            	}
            }
            for(int i = 0;i < FeatureNum;i++)
            {
            	for(int j = 0;j < FeatureNum;j++)
            	{
            		CorMatrix[i,j] = NorStandardMatrix[i,j]/Math.Sqrt(NorStandardMatrix[i,i] *NorStandardMatrix[j,j]);
            	}
            }	
            return CorMatrix;
        }

        public static double[,] EigenValueVector(double[,] CoMatrix)
        {
            int Dim = CoMatrix.GetLength(0);
            double[,] EigenValue = CoMatrix;
            double[,] TempMatrix = EigenValue;
            double maxEV;
            do
            {
                double[] Parameter = new double[4];
                maxEV = EigenValue[0, 1];
                int Row = 0;
                int Colomn = 1;
                for (int i = 0; i < Dim - 1; i++)
                {
                    for (int j = i + 1; j < Dim; j++)
                    {
                        if (EigenValue[i, j] > maxEV)
                        {
                            maxEV = EigenValue[i, j];
                            Row = i;
                            Colomn = j;
                        }
                    }
                }

                Parameter[0] = (EigenValue[Row, Row] - EigenValue[Colomn, Colomn]) / (2 * EigenValue[Row, Colomn]);
                Parameter[1] = Math.Sign(Parameter[0]) * (Math.Sqrt(Math.Pow(Parameter[0], 2) + 1) - Math.Abs(Parameter[0]));
                Parameter[2] = 1 / Math.Sqrt(1 + Math.Pow(Parameter[1], 2));
                Parameter[3] = Parameter[1] * Parameter[2];
                
                EigenValue[Row, Row] = Math.Pow(Parameter[2], 2) * TempMatrix[Row, Row] + Math.Pow(Parameter[3], 2) * TempMatrix[Colomn, Colomn] + 2 * Parameter[3] * Parameter[2] * TempMatrix[Row, Colomn];
                EigenValue[Colomn, Colomn] = Math.Pow(Parameter[3], 2) * TempMatrix[Row, Row] + Math.Pow(Parameter[2], 2) * TempMatrix[Colomn, Colomn] - 2 * Parameter[3] * Parameter[2] * TempMatrix[Row, Colomn];
                EigenValue[Row, Colomn] = 0;
                EigenValue[Colomn, Row] = 0;
                for (int i = 0; i < Dim; i++)
                {
                    if ((i != Row) && (i != Colomn))
                    {
                        EigenValue[Row, i] = Parameter[2] * TempMatrix[Row, i] + Parameter[3] * TempMatrix[Colomn, i];
                        EigenValue[i, Row] = Parameter[2] * TempMatrix[Row, i] + Parameter[3] * TempMatrix[Colomn, i];
                        EigenValue[Colomn, i] = Parameter[2] * TempMatrix[Row, i] + Parameter[3] * TempMatrix[Colomn, i];
                        EigenValue[i, Colomn] = -Parameter[3] * TempMatrix[Row, i] + Parameter[2] * TempMatrix[Colomn, i];
                    }
                }
            }
            while (Math.Abs(maxEV) > Math.Pow(10, -10));
            return EigenValue;
        }
    }  
}
