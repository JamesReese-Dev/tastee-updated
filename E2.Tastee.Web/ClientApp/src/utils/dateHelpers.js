import moment from "moment";

const YMD = "YYYY-MM-DD";
const MDY = "MM/DD/YY";
const MDY4 = "MM/DD/YYYY";
const YMDHMS = "YYYY-MM-DD HH:mm:ss";
const YMDHM = "YYYY-MM-DD HH:mm";
const MDYHMA = "M/D/YY h:mmA";

const momentFromString = function (date, fmt = null) {
  if (fmt) {
    return moment(date, fmt, true);
  }
  if (date) {
    return moment(date);
  }
  return null;
};

const VALID_DATE_FORMAT_LIST = [
  MDY4,
  "M/D/YY",
  "MM/D/YY",
  MDY,
  "MM/D/YYYY",
  "M/D/YYYY",
  YMD,
];

const verifyDate = (s) => {
  if (!s) return null;
  const test = moment(s, VALID_DATE_FORMAT_LIST);
  return test.isValid() ? test : null;
};

export default {
  MDY, YMD, MDY4, YMDHM, MDYHMA, verifyDate,
  isDateValid(dateString) {
    if (!dateString) {
      return false;
    }
    if (!moment(dateString, "MM/DD/YYYY", true).isValid()) {
      return false;
    }
    return true;
  },

  getSelectedDate(date, fmt = null) {
    if (date) {
      return fmt ? moment(date).format(fmt) : moment(date);
    }
    return null;
  },

  toShortDateString(d, fmt = null) {
    if (!d) return "";
    return momentFromString(d, fmt).format("DD-MMM-YY");
  },

  toMDYDateString(d, fmt = null) {
    if (!d) return "";
    return momentFromString(d, fmt).format("MM-DD-YY");
  },

  toDateString(d, fmt = null) {
    if (!d) return "";
    return momentFromString(d, fmt).format("ddd, MM/DD/YY");
  },

  toTimeString(d, fmt = null) {
    if (!d) return "";
    return momentFromString(d, fmt).format("h:mm a");
  },

  getStringFromMoment(m) {
    if (m) {
      return m.format("YYYY-MM-DD");
    }
    return null;
  },

  formatDateToShortDate(date, fmt = null) {
    if (date) {
      if (moment.isMoment(date)) {
        return date.format(MDY4);
      } else if (fmt === null) {
        return moment(date).format(MDY4);
      } else {
        return moment(date, fmt, true).format(MDY4);
      }
    }
    return date;
  },

  getTimeStringFromMoment(m) {
    if (!m) return "";
    return m.format("hh:mm A");
  },

  getMomentFromString(date, fmt) {
    return momentFromString(date, fmt);
  },

  parseDatePickerDate(s, fmt = YMD) {
    const validated = verifyDate(s);
    if (validated) {
      return validated.format(fmt);
    } else {
      return s;
    }
  },

  getTimeRange() {
    const start = moment().startOf("06:00");
    const times = 14 * 2; // 14 hours * two 30 mins sessions/hour

    for (let i = 0; i < times; i++) {
      const toPrint = moment(start)
        .add(30 * i, "minutes")
        .format("hh:mm A");
      return toPrint;
    }
  },

  formatDateForServer(date, fmt) {
    if (date) {
      if (moment.isMoment(date)) return date.format(YMD);
      return moment(date, fmt).format(YMD);
    }
    return date;
  },

  // formatDateForServer(dateString) {
  //     var date = this.momentFromString(dateString);
  //     return this.getStringFromMoment(date);
  // },

  formatDateTimeForServer(dateTime) {
    if (dateTime) {
      if (moment.isMoment(dateTime)) return dateTime.format(YMDHMS);
      return moment(dateTime).format(YMDHMS);
    }
    return dateTime;
  },

  dateFormatForClient(datetime) {
    if (datetime) {
      return moment(datetime).toDate();
    }
  },

  dateTimeFormat(dateTime, fmt = null) {
    if (!fmt)
      fmt = "MM/DD/YYYY hh:mm:ss";
    if (dateTime) {
      return moment.utc(dateTime).local().format(fmt);
    }
    return dateTime;
  },

  monthDayAndYear(dateTime) {
    if (dateTime) {
      return moment.utc(dateTime).local().format(MDY4);
    }
    return dateTime;
  },
  momentFromString: momentFromString
};
