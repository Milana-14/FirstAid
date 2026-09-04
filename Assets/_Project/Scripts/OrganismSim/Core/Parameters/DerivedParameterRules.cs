using System;
using System.Collections.Generic;

namespace OrganismSim.Core
{
    public sealed class BloodPressureRule : IDerivedParameterRule
    {
        public ParameterType Type => ParameterType.BloodPressure;

        public double Compute(IReadOnlyDictionary<ParameterType, double> snapshot, PathologicalState pathology)
        {
            const double baselineMmHg = 118; // balance

            double volumeRatio = snapshot[ParameterType.BloodVolume] / 100.0;
            double pvrRatio = snapshot[ParameterType.PeripheralVascularResistance] / 100.0;
            double hrRatio = HeartRateFactor(snapshot[ParameterType.HeartRate]);
            double contractilityFactor = 1 - pathology.GetSeverity(ConditionType.HeartFailure) / 10.0 * 0.6; // balance

            double pleuralRatio =
                Math.Clamp(
                    snapshot[ParameterType.PleuralBloodVolume] /
                    ParameterRange.All[ParameterType.PleuralBloodVolume].LethalHigh, 0, 1);
            double tamponade = pathology.GetSeverity(ConditionType.PericardialTamponade);
            double obstructiveFactor = Math.Clamp(1 - pleuralRatio * 0.4 - tamponade / 10.0 * 0.7, 0.1, 1); // balance

            return baselineMmHg * volumeRatio * pvrRatio * hrRatio * contractilityFactor * obstructiveFactor;
        }

        private static double HeartRateFactor(double heartRate)
        {
            const double peakHeartRate = 105; // balance - тук CO е максимален
            const double halfWidth = 100; // balance - колко бързо пада встрани от пика
            const double minRatio = 0.2; // balance - под тази стойност не пада (иначе formula дава отрицателно)

            double normalizedDistance = (heartRate - peakHeartRate) / halfWidth;
            double ratio = 1 - normalizedDistance * normalizedDistance;

            return Math.Max(minRatio, ratio);
        }
    }

    public sealed class CerebralBloodFlowRule : IDerivedParameterRule
    {
        public ParameterType Type => ParameterType.CerebralBloodFlow;

        public double Compute(IReadOnlyDictionary<ParameterType, double> snapshot, PathologicalState pathology)
        {
            double cpp = snapshot[ParameterType.BloodPressure] - snapshot[ParameterType.IntracranialPressure];
            double globalFlowFactor = AutoregulationFactor(cpp);

            var co2Range = ParameterRange.All[ParameterType.BloodCo2Level];
            double co2Deficit = Math.Max(0, co2Range.NormalMin - snapshot[ParameterType.BloodCo2Level]);
            double co2Factor = Math.Clamp(1 - co2Deficit / co2Range.NormalMin * 0.3, 0.5, 1); // balance

            var o2Range = ParameterRange.All[ParameterType.BloodOxygenLevel];
            double o2NormalMid = (o2Range.NormalMin + o2Range.NormalMax) / 2.0;
            double oxygenFactor =
                Math.Clamp(snapshot[ParameterType.BloodOxygenLevel] / o2NormalMid, 0.3, 1.1); // balance

            double strokeSeverity = pathology.GetSeverity(ConditionType.ImpairedCerebralBloodFlow);
            double sympatheticSeverity = pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity);
            double parasympatheticSeverity = pathology.GetSeverity(ConditionType.IncreasedParasympatheticActivity);

            double localFlowFactor = 1
                                     - strokeSeverity / 10.0 * 0.7
                                     + sympatheticSeverity / 10.0 * 0.15
                                     - parasympatheticSeverity / 10.0 * 0.15;

            localFlowFactor = Math.Clamp(localFlowFactor, 0, 1.3);

            return globalFlowFactor * co2Factor * oxygenFactor * localFlowFactor * 100;
        }

        private static double AutoregulationFactor(double cpp)
        {
            const double floor = 50; // balance
            const double ceiling = 150; // balance

            if (cpp >= floor && cpp <= ceiling) return 1.0;

            if (cpp < floor)
            {
                double deficit = floor - cpp;
                return Math.Clamp(1.0 - deficit / floor, 0, 1);
            }

            double excess = cpp - ceiling;
            return Math.Clamp(1.0 + excess / ceiling * 0.3, 1, 1.3);
        }
    }

    public sealed class PeripheralOrganBloodFlowRule : IDerivedParameterRule
    {
        public ParameterType Type => ParameterType.PeripheralOrganBloodFlow;

        public double Compute(IReadOnlyDictionary<ParameterType, double> snapshot, PathologicalState pathology)
        {
            const double normalBpMmHg = 105; // balance

            double globalFlowFactor = Math.Clamp(snapshot[ParameterType.BloodPressure] / normalBpMmHg, 0, 1.3);

            var o2Range = ParameterRange.All[ParameterType.BloodOxygenLevel];
            double o2NormalMid = (o2Range.NormalMin + o2Range.NormalMax) / 2.0;
            double oxygenFactor =
                Math.Clamp(snapshot[ParameterType.BloodOxygenLevel] / o2NormalMid, 0.3, 1.1); // balance
            double sympatheticSeverity = pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity);
            double parasympatheticSeverity = pathology.GetSeverity(ConditionType.IncreasedParasympatheticActivity);

            double localFlowFactor = 1
                                     - sympatheticSeverity / 10.0 * 0.25 // balance
                                     + parasympatheticSeverity / 10.0 * 0.25; // balance

            localFlowFactor = Math.Clamp(localFlowFactor, 0, 1.3);

            return globalFlowFactor * oxygenFactor * localFlowFactor * 100;
        }
    }
}