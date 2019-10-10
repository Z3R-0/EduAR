public enum PreferenceProperties {
    Layout,
    Language
}

public enum Layout {
    Normal,
    RightSide,
    Weird
}

public enum Language {
    nl_NL,
    en_EN
}

public class Preference {
    public Layout Layout { get; set; }
    public Language Language { get; set; }
}
