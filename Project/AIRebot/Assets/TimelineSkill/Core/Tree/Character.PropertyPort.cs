using System;
using TreeDesigner;
using Taco.Timeline;

[Serializable]
[PropertyColor(239, 163, 146)]
public class TimelinePropertyPort : PropertyPort<Timeline>
{
    public TimelinePropertyPort() { }
}

[Serializable]
[PropertyColor(239, 163, 146)]
public class TimelineExposedProperty : BaseExposedProperty<Timeline>
{
    public TimelineExposedProperty() { }
}

[Serializable]
[PropertyColor(239, 163, 146)]
public class AbilityPropertyPort : PropertyPort<Ability>
{
    public AbilityPropertyPort() { }
}

[Serializable]
[PropertyColor(239, 163, 146)]
public class AbilityExposedProperty : BaseExposedProperty<Ability>
{
    public AbilityExposedProperty() { }
}