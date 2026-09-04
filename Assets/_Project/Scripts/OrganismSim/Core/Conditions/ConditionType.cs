namespace OrganismSim.Core
{
    public enum ConditionType
    {
        ExternalBleeding, // външно кървене
        Hemothorax, // вътрешно кървене
        IntraAbdominalBleeding, // вътрешно кървене
        PericardialTamponade, // вътрешно кървене
        IntracranialProcess, // вътречерепен процес
        AirwayObstruction, // запушване на дихателните пътища
        LungInjury, // увреждане на белите дробове
        PulmonaryEdema, // белодробен оток
        HeartFailure, // сърдечна недостатъчност
        ImpairedCerebralBloodFlow, // нарушение на мозъчното кръвоснабдяване
        Shock, // шок
        Hypoglycemia, // хипогликемия
        Hyperglycemia, // хипергликемия
        Dehydration, // обезводняване
        Seizure, // гърч - самозатихващо се, за разлика от останалите

        IncreasedSympatheticActivity, // увеличаване на симпатиковата активност
        IncreasedParasympatheticActivity, // увеличаване на парасимпатиковата активност

        Hypothermia,
        Hyperthermia,
        MildAllergicReaction,
        Anaphylaxis,

        Fracture111, // счупена кост - трябва да се промени
    }
}