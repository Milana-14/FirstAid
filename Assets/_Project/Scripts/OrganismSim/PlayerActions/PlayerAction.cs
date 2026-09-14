using System;
using OrganismSim.Core;

namespace OrganismSim.PlayerActions
{
    public sealed class GiveWater : IPlayerAction
    {
        public string NameKey => "action_give_water_name";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5);

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6);
                patient.Exposure.RecordChoking();
                
                // Передаем ключ сообщения и имя пациента в качества аргумента
                return new ActionResult(
                    ActionOutcome.Complication, 
                    "action_give_water_choked", 
                    new object[] { new { patientName = patient.Name } }
                );
            }

            patient.Pathology.Adjust(ConditionType.Dehydration, -3);
            return new ActionResult(ActionOutcome.Success, "action_give_water_success");
        }
    }

    public sealed class GiveJuice : IPlayerAction
    {
        public string NameKey => "action_give_juice_name";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5);

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6);
                patient.Exposure.RecordChoking();
                return new ActionResult(
                    ActionOutcome.Complication, 
                    "action_give_juice_choked", 
                    new object[] { new { patientName = patient.Name } }
                );
            }

            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, 6);
            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 32, 50);

            return new ActionResult(ActionOutcome.Success, "action_give_juice_success");
        }
    }

    public sealed class GiveBiscuit : IPlayerAction
    {
        public string NameKey => "action_give_biscuit_name";

        public ActionResult Execute(Patient patient)
        {
            double consciousness = patient.Physiology.Get(ParameterType.Consciousness);

            double impairment = Math.Clamp((70 - consciousness) / 70.0, 0, 1);
            double chokeProbability = consciousness <= 2 ? 1.0 : Math.Pow(impairment, 1.5);

            if (new Random().NextDouble() < chokeProbability)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, chokeProbability * 6);
                patient.Exposure.RecordChoking();
                return new ActionResult(
                    ActionOutcome.Complication, 
                    "action_give_biscuit_choked", 
                    new object[] { new { patientName = patient.Name } }
                );
            }

            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 34, 70);
            return new ActionResult(ActionOutcome.Success, "action_give_biscuit_success");
        }
    }

    public sealed class GiveGlucagon : IPlayerAction
    {
        public string NameKey => "action_give_glucagon_name";

        public ActionResult Execute(Patient patient)
        {
            const double vomitProbability = 0.15;

            bool vomits = new Random().NextDouble() < vomitProbability;
            if (vomits && patient.Physiology.Get(ParameterType.Consciousness) < 15)
            {
                patient.Pathology.Adjust(ConditionType.AirwayObstruction, 4);
                patient.Exposure.RecordChoking();
            }

            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, 45 * 0.7, 80);
            patient.Pathology.Adjust(ConditionType.IncreasedSympatheticActivity, 1.5);

            return vomits
                ? new ActionResult(
                    ActionOutcome.Complication, 
                    "action_give_glucagon_vomit", 
                    new object[] { new { patientName = patient.Name } }
                )
                : new ActionResult(ActionOutcome.Success, "action_give_glucagon_success");
        }
    }

    public sealed class GiveInsulin : IPlayerAction
    {
        public string NameKey => "action_give_insulin_name";

        public ActionResult Execute(Patient patient)
        {
            const double absorptionWindowSeconds = 120;

            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, -10);
            patient.Absorptions.Enqueue(ParameterType.BloodGlucoseLevel, -50, absorptionWindowSeconds);

            return new ActionResult(ActionOutcome.Success, "action_give_insulin_success");
        }
    }

    public sealed class HelpStand : IPlayerAction
    {
        public string NameKey => "action_help_stand_name";

        public ActionResult Execute(Patient patient)
        {
            if (patient.Posture == PatientPosture.Standing)
                return new ActionResult(ActionOutcome.Success, "action_help_stand_already_standing");

            if (patient.Physiology.Get(ParameterType.Consciousness) <= 2)
                return new ActionResult(ActionOutcome.Blocked, "action_help_stand_unconscious");

            patient.Posture = PatientPosture.Standing;
            return new ActionResult(ActionOutcome.Success, "action_help_stand_success");
        }
    }

    public sealed class HelpSit : IPlayerAction
    {
        public string NameKey => "action_help_sit_name";

        public ActionResult Execute(Patient patient)
        {
            patient.Posture = PatientPosture.Supine;
            return new ActionResult(ActionOutcome.Success, "action_help_sit_success");
        }
    }
}