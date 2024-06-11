namespace TrainStation;

enum WagonType
{
    Locomotive,
    Passenger,
    FreightClosed,
    FreightOpen,
    CarTransporter
}

record Wagon(WagonType Type, string WagonNumber = "");
