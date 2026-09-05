using System;
using OrganismSim.Core;

namespace OrganismSim.PlayerActions
{
    public sealed class GiveWater : IPlayerAction
    {
        public string Name => "Дай вода";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5); // balance

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6); // balance
                patient.Exposure.RecordChoking();
                return new ActionResult(ActionOutcome.Complication, "Бабата се задавя с водата.");
            }

            patient.Pathology.Adjust(ConditionType.Dehydration, -3);
            return new ActionResult(ActionOutcome.Success, "Дадохте вода.");
        }
    }

    public sealed class GiveJuice : IPlayerAction
    {
        public string Name => "Дай 150 мл сок";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5); // balance

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6); // balance
                patient.Exposure.RecordChoking();
                return new ActionResult(ActionOutcome.Complication, "Бабата се задавя със сока.");
            }

            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, 6); // balance
            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 32, 30); // balance

            return new ActionResult(ActionOutcome.Success, "Дадохте 150 мл сок.");
        }
    }

    public sealed class GiveBiscuit : IPlayerAction
    {
        public string Name => "Дай бисквитка";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5); // balance

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6); // balance
                patient.Exposure.RecordChoking();
                return new ActionResult(ActionOutcome.Complication, "Бабата се задавя с бисквитката.");
            }

            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 34, 60); // balance
            return new ActionResult(ActionOutcome.Success, "Дадохте бисквитка.");
        }
    }

    public sealed class GiveGlucagon : IPlayerAction
    {
        public string Name => "Дай глюкагон (инжекция)";

        public ActionResult Execute(Patient patient)
        {
            const double vomitProbability = 0.15; // balance

            bool vomits = new Random().NextDouble() < vomitProbability;
            if (vomits && patient.Physiology.Get(ParameterType.Consciousness) < 15)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, 4); // balance
                patient.Exposure.RecordChoking();
            }

            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 45 * 0.7, 90); // balance
            patient.Pathology.Adjust(ConditionType.IncreasedSympatheticActivity, 1.5); // balance

            return vomits
                ? new ActionResult(ActionOutcome.Complication, "Бабата повръща след инжекцията.")
                : new ActionResult(ActionOutcome.Success, "Инжектирахте глюкагон.");
        }
    }
    
    public sealed class GiveInsulin : IPlayerAction
    {
        public string Name => "Дай инсулин";

        public ActionResult Execute(Patient patient)
        {
            const double absorptionWindowSeconds = 120; // balance

            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, -10); // balance
            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, -50, absorptionWindowSeconds);

            return new ActionResult(ActionOutcome.Success, "Дадохте инсулин.");
        }
    }
    
    public sealed class HelpStand : IPlayerAction
    {
        public string Name => "Помогни да стане";

        public ActionResult Execute(Patient patient)
        {
            if (patient.Posture == PatientPosture.Standing)
                return new ActionResult(ActionOutcome.Success, "Тя вече стои права.");

            if (patient.Physiology.Get(ParameterType.Consciousness) <= 2)
                return new ActionResult(ActionOutcome.Blocked, "Тя е в безсъзнание — не може да стане.");

            patient.Posture = PatientPosture.Standing;
            return new ActionResult(ActionOutcome.Success, "Тя стана права с ваша помощ.");
        }
    }
    
    public sealed class HelpSit : IPlayerAction
    {
        public string Name => "Помогни да седне";

        public ActionResult Execute(Patient patient)
        {
            patient.Posture = PatientPosture.Supine;
            return new ActionResult(ActionOutcome.Success, "Настанихте я седнала.");
        }
    }
}