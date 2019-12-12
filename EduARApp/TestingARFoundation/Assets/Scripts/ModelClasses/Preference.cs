public enum PreferenceProperties {
    Id,
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
    public int Id { get; set; }
    public Layout Layout { get; set; }
    public Language Language { get; set; }

    public Preference() {
        Layout = Layout.Normal;
        Language = Language.nl_NL;
    }
}
