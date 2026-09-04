namespace OrganismSim.Core
{
    public sealed class ExposureLog
    {
        public double SecondsSevereBrainDeficit { get; private set; }
        public int ChokingEvents { get; private set; }
        public int SeizureEvents { get; private set; }
        public int FallingEvents { get; private set; }
        public int MyocardialInfractionEvents { get; private set; }
        public int StrokeEvents { get; private set; }
        public int LethalArrhythmiaEvents { get; private set; }

        public void ApplyTick(Patient patient, int seconds)
        {
            if (patient.Physiology.Get(ParameterType.BrainFunction) < 40)
                SecondsSevereBrainDeficit += seconds;
        }

        public void RecordChoking() => ChokingEvents++;
        public void RecordSeizure() => SeizureEvents++;
        public void RecordFallings() => FallingEvents++;
        public void RecordMyocardialInfraction() => MyocardialInfractionEvents++;
        public void RecordStroke() => StrokeEvents++;
        public void RecordLethalArrhythmia() => LethalArrhythmiaEvents++;
    }
}