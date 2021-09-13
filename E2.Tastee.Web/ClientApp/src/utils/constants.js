import _ from "lodash";

const THEMES = {
  darkTheme: "darkTheme",
  lightTheme: "lightTheme",
  default: "lightTheme",
};

const BREAKPOINTS = {
  mobile: 320,
  mobileLandscape: 480,
  smallDesktop: 768,
  tablet: 856,
  tabletLandscape: 992,
  desktop: 1080,
  desktopLarge: 1500,
  desktopWide: 1920,
};

const PAGE_NAMES = {
  HOME: "Home",
};

const SORT_DIRECTION = {
  ASC: "Ascending",
  DESC: "Descending",
};

const REFERENCE_DATA_URL_LIST = [
  {
    maxNameLength: 50,
    elementName: "SomeElement",
    pageTitle: "Some Elements",
    toggleURL: "Reference/ToggleSomeElementActive",
    saveURL: "Reference/SaveSomeElement",
    listURL: "Reference/SomeList",
    reactPath: "/some-elements",
  },
];

const STATES = [
  "AL",
  "AK",
  "AR",
  "AZ",
  "CA",
  "CO",
  "CT",
  "FL",
  "DE",
  "GA",
  "ID",
  "HI",
  "IL",
  "IN",
  "IA",
  "KS",
  "KY",
  "LA",
  "ME",
  "MD",
  "MA",
  "MI",
  "MN",
  "MS",
  "MO",
  "MT",
  "NE",
  "NV",
  "NH",
  "NJ",
  "NM",
  "NY",
  "NC",
  "ND",
  "OH",
  "OK",
  "OR",
  "PA",
  "RI",
  "SC",
  "SD",
  "TN",
  "TX",
  "UT",
  "VT",
  "VA",
  "WA",
  "WV",
  "WI",
  "WY",
];

const ROLE_NAMES = {
  ADMIN: "Administrator",
  TASTER: "Taster"
};

const ROLE_IDS = {
  TASTER: 2,
  ADMIN: 1
};

const ROLE_DICTIONARY = {
  2: ROLE_NAMES.TASTER,
  1: ROLE_NAMES.ADMIN,
};

const ALL_ROLES = [
  {
    value: ROLE_IDS.ADMIN,
    label: ROLE_NAMES.ADMIN,
    groupName: "Administration"
  },
  {
    value: ROLE_IDS.TASTER,
    label: ROLE_NAMES.TASTER,
    groupName: "Administration"
  }
];

const EMPTY_USER = {
  id: -1,
  firstName: "",
  middleName: "",
  lastName: "",
  mothersMaidenName: '',
  phoneNumber: "",
  email: "",
  username: "",
  roles: [],
  editing: true,
  password: "",
};

const BloodTypes = [
  {
    label: "a+",
    value: "A+"
  },
  {
    label: "a-",
    value: "A-"
  },
  {
    label: "b+",
    value: "B+"
  },
  {
    label: "b-",
    value: "B-"
  },
  {
    label: "ab+",
    value: "AB+"
  },
  {
    label: "ab",
    value: "AB-"
  },
  {
    label: "o+",
    value: "O+"
  },
  {
    label: "o-",
    value: "O-"
  },
  {
    label: "non-human",
    value: "Non-Human"
  }

];

const constants = {
  THEMES,
  PAGE_NAMES,
  ZIP_CODE_REGEX: /^[0-9]{5}$/,
  US_STATE_LIST: [
    "Alabama",
    "Alaska",
    "American Samoa",
    "Arizona",
    "Arkansas",
    "California",
    "Colorado",
    "Connecticut",
    "Delaware",
    "District of Columbia",
    "Federated States of Micronesia",
    "Florida",
    "Georgia",
    "Guam",
    "Hawaii",
    "Idaho",
    "Illinois",
    "Indiana",
    "Iowa",
    "Kansas",
    "Kentucky",
    "Louisiana",
    "Maine",
    "Marshall Islands",
    "Maryland",
    "Massachusetts",
    "Michigan",
    "Minnesota",
    "Mississippi",
    "Missouri",
    "Montana",
    "Nebraska",
    "Nevada",
    "New Hampshire",
    "New Jersey",
    "New Mexico",
    "New York",
    "North Carolina",
    "North Dakota",
    "Northern Mariana Islands",
    "Ohio",
    "Oklahoma",
    "Oregon",
    "Palau",
    "Pennsylvania",
    "Puerto Rico",
    "Rhode Island",
    "South Carolina",
    "South Dakota",
    "Tennessee",
    "Texas",
    "Utah",
    "Vermont",
    "Virgin Island",
    "Virginia",
    "Washington",
    "West Virginia",
    "Wisconsin",
    "Wyoming",
  ],
  PASSWORD_COMPLEXITY_DESCRIPTION:
    "Passwords must be at least 8 characters long and contain at least 1 uppercase character, at least 1 lowercase character and at least 1 number.",
  XLSX_MIME: "application/vnd.openxmlformats-officedocument.spreadsheetml",
  CSV_MIME: "text/csv",
  TEN_SECONDS_MS: 10000,
  FIVE_MINUTES_MS: 300000,
  TEN_MINUTES_MS: 600000,
  TWENTY_MINUTES_MS: 1200000,
  TWENTY_THREE_HOURS_MS: 82800000,
  STATES: _.map(STATES, (s) => ({ label: s, value: s })),
  SORT_DIRECTION,
  ROLE_NAMES,
  ROLE_IDS,
  DEFAULT_TIMEZONE: "Eastern Standard Time",
  ROLE_DICTIONARY,
  BREAKPOINTS,
  ALL_ROLES,
  REFERENCE_DATA_URL_LIST,
  EMPTY_USER,
  COLOR_OPTIONS: [
    {value: 0, label: "Gray", hexCode: "#A7B0B7"},
    {value: 1, label: "Purple", hexCode: "#303082"},
    {value: 2, label: "Orange", hexCode: "#DF7953"},
    {value: 3, label: "Light Purple", hexCode: "#6E6EA8"},
    {value: 4, label: "Green", hexCode: "#799E51"},
    {value: 5, label: "Blue", hexCode: "#023f72"},
    {value: 6, label: "Light Blue", hexCode: "#7C9EB2"},
  ]
};

export default constants;
