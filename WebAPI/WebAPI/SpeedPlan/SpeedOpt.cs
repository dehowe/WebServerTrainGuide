using WebAPI.Function;

namespace WebAPI.SpeedPlan
{
    public class SpeedOpt
    {
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        // 外部变量
        private int debugFlag = 1;                                      // 调试标识，0：无 1：打印过程数据
        private UInt16 speed = 0;                                       // 初始速度
        private UInt16 targetTime = 0;                                  // 目标运行时分
        private UInt32 intervalLength = 0;                              // 区间长度
        private List<float> gradient = new List<float>();               // 区间坡度
        private byte speedLimitNum = 0;                                 // 限速转换点
        private List<UInt32> speedLimitLoc = new List<UInt32>();        // 限速转换点位置
        private List<UInt16> speedLimitValue = new List<UInt16>();      // 限速转换点限速值
        private byte solveDim = 0;                                      // 解维度
        private List<UInt32> upBound = new List<UInt32>();              // 求解上边界
        private List<UInt32> downBound = new List<UInt32>();            // 求解下边界
        private List<byte> switchFlag = new List<byte>();               // 工况切换标志 1:牵引 2：制动 3：惰行
        // 局部变量
        public UInt16 discreteSize = 100;                              // 离散大小1m
        private float tractionRatio = 0.8f;                             // 速度规划牵引输出比率
        private float brakeRatio = 0.3f;                                // 速度规划制动输出比率
        private UInt16 dim = 0;                                         // 维度
        private UInt32 remainLength = 0;                                // 区间进行等间隔离散后的剩余长度
        private List<UInt16> speedLimitMax = new List<UInt16>();        // 最速曲线
        private List<UInt16> speedLimitMMax = new List<UInt16>();       // 顶棚限速
        public List<UInt16> optimalSpeed = new List<UInt16>();         // 优化速度
        public List<UInt16> levelFlag = new List<UInt16>();            // 优化工况

        public void baseDataInit(UInt16 speed, UInt16 targetTime, UInt32 intervalLength, List<float> gradient, UInt32[,] limit)
        {
            this.speed = speed;
            this.targetTime = targetTime;
            this.intervalLength = intervalLength;
            this.gradient = gradient;
            this.speedLimitNum = (byte)limit.GetLength(1);
            // 取限速转换点信息
            for (int i = 0; i < this.speedLimitNum; i++)
            {
                speedLimitLoc.Add(limit[0, i]);   // 限速转换点位置
                speedLimitValue.Add((UInt16)limit[1, i]); // 限速转换点限速值

            }
            // 启发设置
            this.solveDim = 1;
            this.downBound.Add(0);
            this.upBound.Add(this.intervalLength);
            this.switchFlag.Add(1);
        }

        // 单车速度优化
        public void SingleSpeedOpt()
        {
            // 建模
            Model();
            // 求解
            GWOSolution();
        }

        //计算防护曲线的开始点
        private UInt16 getEbiEnd(UInt16 ebiBegin, List<UInt16> spdTemp, int index)
        {
            UInt16 vEnd = ebiBegin;
            // 找到上一限速下降点
            for (int i = 0; i < index - 1; i++)
            {
                if (spdTemp[index - i] >= vEnd)
                {
                    vEnd = spdTemp[index - i];
                }
                else
                {
                    break;
                }
            }
            return vEnd;
        }

        // 根据区间限速计算防护速度
        private void getEbi(UInt16 spdBegin, UInt16 spdEnd, UInt16 index, ref List<UInt16> spdLimit)
        {
            float accBrake = 0f; // 制动减速度临时变量
            UInt16 spdTemp = 0; // 速度临时变量
            UInt16 vIndex = spdBegin;  // 速度索引
            while (vIndex < spdEnd)
            {
                accBrake = getBrakeAcc(vIndex);
                spdTemp = (UInt16)Math.Sqrt(vIndex * vIndex + 2 * accBrake * this.discreteSize);
                if (spdTemp < spdEnd)
                {
                    if (index > 0)
                    {
                        if (spdTemp < spdLimit[index])
                        {
                            spdLimit[index] = spdTemp;
                        }
                        vIndex = spdTemp;
                        index--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // 计算最速曲线
        private void getMaxSpeedLimit()
        {
            List<UInt16> speedLimitTemp = new List<UInt16>();    // 限速临时存储
            List<UInt16> speedLimitMMax = new List<UInt16>();    // 顶棚限速
            List<UInt16> limitFallBegin = new List<UInt16>();    // 限速下降开始点
            List<UInt16> limitFallEnd = new List<UInt16>();      // 限速下降结束点
            List<UInt16> limitFallIndex = new List<UInt16>();    // 限速下降点索引
            int index = 0;
            UInt16 ebiEndTemp = 0; // ebi结束点临时变量
            // 提取限速下降点
            for (int i = 0; i < this.dim + 1; i++)
            {
                speedLimitTemp.Add(this.speedLimitValue[index]);
                speedLimitMMax.Add(this.speedLimitValue[index]);
                if (this.discreteSize * (i + 1) <= this.speedLimitLoc[index])
                {
                    continue;
                }
                else
                {
                    index++;
                    if (index < this.speedLimitNum)
                    {
                        if (this.speedLimitValue[index] < this.speedLimitValue[index - 1])
                        {
                            ebiEndTemp = getEbiEnd(this.speedLimitValue[index], speedLimitTemp, i);
                            limitFallBegin.Add(ebiEndTemp);
                            limitFallEnd.Add(this.speedLimitValue[index]);
                            limitFallIndex.Add((UInt16)i);
                        }
                    }
                    else
                    {
                        ebiEndTemp = getEbiEnd(0, speedLimitTemp, i);
                        limitFallBegin.Add(ebiEndTemp);
                        limitFallEnd.Add(0);
                        limitFallIndex.Add((UInt16)(i - 1));
                    }
                }
            }
            speedLimitTemp[this.dim] = 0;
            speedLimitMMax[this.dim] = 0;
            // 根据限速下降计算目标距离防护速度
            for (int i = 0; i < limitFallIndex.Count; i++)
            {
                getEbi(limitFallEnd[i], limitFallBegin[i], limitFallIndex[i], ref speedLimitTemp);
            }
            // 返回参数
            this.speedLimitMax = speedLimitTemp;
            this.speedLimitMMax = speedLimitMMax;

            // Debug打印
            //if (this.debugFlag == 1)
            //{
            //    for (int i = 0; i < this.speedLimitMax.Count; i++)
            //    {
            //        Console.WriteLine("静态限速:{0}, 防护速度:{1}", this.speedLimitMMax[i], this.speedLimitMax[i]);
            //    }
            //}
        }

        // 建模
        private void Model()
        {
            // 计算离散维度和离散剩余长度
            this.dim = Convert.ToUInt16(Math.Ceiling((double)this.intervalLength / this.discreteSize));
            this.remainLength = this.intervalLength - (UInt32)this.dim * this.discreteSize;
            // 计算最速曲线
            getMaxSpeedLimit();
        }

        // 根据速度查询牵引减速度
        private float getTractionAcc(UInt16 speed)
        {
            return 86 * this.tractionRatio;
        }

        // 根据速度查询制动减速度
        private float getBrakeAcc(UInt16 speed)
        {
            return 146 * brakeRatio;
        }

        // 根据位置查询坡度加速度
        private float getGradientAcc(UInt32 dis)
        {
            int disIndex = (int)dis / this.discreteSize;
            float gradient = 0, accGradient = 0;
            if (disIndex < this.gradient.Count)
            {
                gradient = this.gradient[disIndex]; // 查询坡度值
            }
            accGradient = 9.8f * gradient / 1000;  //坡度附加加速度
            return accGradient;
        }

        //初始化狼群位置分布
        private UInt32[,] Initialize(int wolvesNum)
        {
            UInt32[,] positions = new UInt32[wolvesNum, this.solveDim];
            Random random = new Random();
            for (int i = 0; i < wolvesNum; i++)
            {
                for (int j = 0; j < this.solveDim; j++)
                {
                    positions[i, j] = (UInt32)random.Next((int)this.downBound[j], (int)this.upBound[j]);
                }

            }
            return positions;
        }

        private UInt16 getSpdNext(UInt16 vIndex, float accIndex)
        {
            UInt16 vNext = 1;
            if (vIndex * vIndex + 2 * accIndex * this.discreteSize > 0)
            {
                vNext = (UInt16)Math.Sqrt(vIndex * vIndex + 2 * accIndex * this.discreteSize);
            }
            return vNext;
        }

        //计算适应度函数
        private float getFitness(UInt32[] position, ref List<UInt16> optimalSpeed, ref List<UInt16> levelFlag, ref float timeSum)
        {
            UInt16 vIndex = this.speed; // 初始速度
            UInt16 vNext = 0;  //下一索引速度
            int posIndex = 0;  // 当前解索引
            timeSum = 0f;
            optimalSpeed.Clear(); // 优化速度存储区清空
            levelFlag.Clear();
            float accGradient = 0, accTraction = 0, accBrake = 0, accIndex = 0, fitness = 65535;
            for (int i = 0; i < this.dim; i++)
            {
                accGradient = getGradientAcc((UInt32)i * this.discreteSize);
                accTraction = getTractionAcc(vIndex);
                accBrake = getGradientAcc(vIndex);
                if (i * discreteSize <= position[posIndex])  // 牵引-惰行
                {
                    if (this.switchFlag[posIndex] == 1)
                    {
                        accIndex = accTraction - accGradient;
                        vNext = (UInt16)Math.Sqrt(vIndex * vIndex + 2 * accIndex * this.discreteSize);
                        levelFlag.Add(1);
                    }
                    else if (this.switchFlag[posIndex] == 2)
                    {
                        accIndex = -accBrake - accGradient;
                        vNext = getSpdNext(vIndex, accIndex);
                        levelFlag.Add(2);
                    }
                    else
                    {
                        accIndex = -accGradient;
                        vNext = getSpdNext(vIndex, accIndex);
                        levelFlag.Add(3);
                    }
                    while ((i + 1) * this.discreteSize > position[posIndex] && posIndex < this.solveDim - 1)
                    {
                        posIndex++;
                    }
                }
                else
                {
                    accIndex = -accGradient;
                    vNext = getSpdNext(vIndex, accIndex);
                    levelFlag.Add(3);
                }
                // 边界约束
                if (vNext > this.speedLimitMax[i + 1])
                {
                    vNext = this.speedLimitMax[i + 1];
                    levelFlag[i] = 2;
                }
                if (i == dim - 1)
                {
                    timeSum = timeSum + 2.0f * (this.discreteSize + this.remainLength) / (vIndex + vIndex);
                }
                else
                {
                    timeSum = timeSum + 2.0f * this.discreteSize / (vIndex + vNext);
                }
                optimalSpeed.Add(vIndex);
                vIndex = vNext;
            }
            optimalSpeed.Add(0);
            levelFlag.Add(2);
            fitness = Math.Abs(timeSum - this.targetTime); //适应度为运行时分差
            return fitness;
        }

        private UInt32[] GetRowArray(UInt32[,] array, int index)
        {
            UInt32[] rowArray = new UInt32[array.GetLength(1)];
            for (int i = 0; i < array.GetLength(1); i++)
            {
                rowArray[i] = array[index, i];
            }
            return rowArray;
        }

        // 相同维度的数组赋值
        private void FillArray(UInt32[] orginArray, ref UInt32[] targetArray)
        {
            for (int i = 0; i < orginArray.Length; i++)
            {
                targetArray[i] = orginArray[i];
            }
        }

        // 灰狼优化算法求解最优工况转换点
        private void GWOSolution()
        {
            int WolvesNum = 20; // 狼群大小
            int iteration = 30; // 迭代次数
            // 初始化Alpha,Beta,Delta狼位置
            UInt32[] positionAlpha = new UInt32[this.solveDim];
            UInt32[] positionBeta = new UInt32[this.solveDim];
            UInt32[] positionDelta = new UInt32[this.solveDim];
            // 初始化狼适应度
            float scoreAlpha = 65535;
            float scoreBeta = 65535;
            float scoreDelta = 65535;
            float convergenceFactor = 1;  // 收敛因子初始值
            // GWO求解过程参数
            float fitness = 65535;
            float timeSum = 0;
            float a = 0;
            double A1, A2, A3;
            double C1, C2, C3;
            double X1, X2, X3;
            double D_alpha, D_beta, D_delta;
            List<UInt16> optimalSpeed = new List<UInt16>();
            List<UInt16> levelFlag = new List<UInt16>();
            UInt32[,] positions = Initialize(WolvesNum); //初始化狼群位置分布
            UInt32[] positionTemp;
            Random random = new Random();
            for (int i = 0; i < iteration; i++)
            {
                for (int j = 0; j < WolvesNum; j++)
                {
                    positionTemp = GetRowArray(positions, j);
                    fitness = getFitness(positionTemp, ref optimalSpeed, ref levelFlag, ref timeSum);
                    if (fitness <= scoreAlpha)
                    {
                        scoreAlpha = fitness;
                        FillArray(positionTemp, ref positionAlpha);
                    }
                    if (fitness > scoreAlpha && fitness <= scoreBeta)
                    {
                        scoreBeta = fitness;
                        FillArray(positionTemp, ref positionBeta);
                    }
                    if (fitness > scoreBeta && fitness <= scoreDelta)
                    {
                        scoreDelta = fitness;
                        FillArray(positionTemp, ref positionDelta);
                    }
                }
                a = a < 0 ? 0 : convergenceFactor - (2.0f / iteration) * i;  //收敛因子随着迭代过程进行，由初始值线性减小到0
                for (int m = 0; m < WolvesNum; m++)
                {
                    for (int n = 0; n < this.solveDim; n++)
                    {
                        // r2 = random.random()
                        A1 = (float)2 * a * random.NextDouble() - a;  // 计算系数A，Equation (3.3)
                        C1 = 2 * random.NextDouble();  // 计算系数C，Equation (3.4)
                                                       // Alpha狼位置更新
                        D_alpha = Math.Abs(C1 * positionAlpha[n] - positions[m, n]);
                        X1 = positionAlpha[n] - A1 * D_alpha;

                        A2 = 2 * a * random.NextDouble() - a;
                        C2 = 2 * random.NextDouble();
                        // Beta狼位置更新
                        D_beta = Math.Abs(C2 * positionBeta[n] - positions[m, n]);
                        X2 = positionBeta[n] - A2 * D_beta;

                        A3 = 2 * a * random.NextDouble() - a;
                        C3 = 2 * random.NextDouble();
                        // Delta狼位置更新
                        D_delta = Math.Abs(C3 * positionDelta[n] - positions[m, n]);
                        X3 = positionDelta[n] - A3 * D_delta;

                        positions[m, n] = (UInt32)((X1 + X2 + X3) / 3);
                        // 边界判断
                        if (positions[m, n] > this.upBound[n])
                            positions[m, n] = this.upBound[n];
                        if (positions[m, n] < this.downBound[n])
                            positions[m, n] = this.downBound[n];

                    }
                }

            }

            // 迭代结束
            fitness = getFitness(positionAlpha, ref optimalSpeed, ref levelFlag, ref timeSum);
            this.optimalSpeed = optimalSpeed;
            this.levelFlag = levelFlag;
            if (debugFlag == 1)
            {
                Log.Info("计划运行时分:{0}s,优化运行时分:{1}s", this.targetTime, timeSum);
                Console.WriteLine("计划运行时分:{0}s,优化运行时分:{1}s", this.targetTime, timeSum);
                for (int i = 0; i < this.optimalSpeed.Count; i++)
                {
                    Log.Info("位置:{0},静态限速:{1}, 防护速度:{2},优化速度:{3}", this.discreteSize * i, this.speedLimitMMax[i], this.speedLimitMax[i], this.optimalSpeed[i]);
                    Console.WriteLine("位置:{0},静态限速:{1}, 防护速度:{2},优化速度:{3}", this.discreteSize * i, this.speedLimitMMax[i], this.speedLimitMax[i], this.optimalSpeed[i]);
                }
            }
        }
    }
    public class SpeedOptManager
    {

        public static void SpeedOptStart(object obj)
        {
            // 将传递的参数转换回具体类型
            SpeedOptParameter speedOptParameter = (SpeedOptParameter)obj;
            SpeedOpt sp = new SpeedOpt();
            sp.baseDataInit(speedOptParameter.speed, speedOptParameter.targetTime, speedOptParameter.intervalLength, speedOptParameter.gradient, speedOptParameter.limit);
            sp.SingleSpeedOpt();
            // 优化曲线赋值
            if(speedOptParameter.CarCode!=0)
            {
                for(int i=0;i<GV.trainOperationInfo.Count;i++)
                {
                    // 找到要赋值曲线的车辆
                    if(GV.trainOperationInfo[i].CarCode == speedOptParameter.CarCode)
                    {
                        GV.trainOperationInfo[i].OptimalLoc.Clear(); 
                        GV.trainOperationInfo[i].OptimalSpeed.Clear();
                        GV.trainOperationInfo[i].LevelFlag.Clear();
                        for (int j=0;j<sp.optimalSpeed.Count;j++)
                        {
                            GV.trainOperationInfo[i].OptimalLoc.Add(j * sp.discreteSize);
                            GV.trainOperationInfo[i].OptimalSpeed.Add(sp.optimalSpeed[j]);
                            GV.trainOperationInfo[i].LevelFlag.Add(sp.levelFlag[j]);
                        }
                        GV.trainOperationInfo[i].OfflineSpeedOptFlag = 2; // 曲线优化完成
                        GV.trainOperationInfo[i].SpeedOptStationCode = GV.trainOperationInfo[i].NextStationCode; // 优化曲线
                        break;
                    }

                }

            }
        }

    }

    public class SpeedOptParameter
    {
        public UInt16 speed { get; set; }
        public UInt16 targetTime { get; set; }
        public UInt32 intervalLength { get; set; }
        public List<float> gradient { get; set; }
        public UInt32[,] limit { get; set; }

        public int CarCode { get; set; }
    }







}