import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Badge } from "reactstrap";
import _ from "lodash";
import { dateHelpers, constants, storage } from "./";
import Papa from "papaparse";

const FOOTER_ID = 'layout-footer';
const BADGE_STYLE = {fontSize: "1.1rem"};

const hideFooter = () => {
  setTimeout(() => {
    const f = document.getElementById('layout-footer');
    try {
      if (f) {
        f.style.display = 'none';
      }
    } catch {}
  }, 200);
};

const setFooterFixedBottom = () => {
  setTimeout(() => {
    const f = document.getElementById(FOOTER_ID);
    try {
      if (f) {
        f.style.display = 'block';
        // const arr = f.className.split(" ");
        // if (arr.indexOf(FOOTER_FLOAT_CLASS) === -1) {
        //   f.className += " " + FOOTER_FLOAT_CLASS;
        // }
      }
    } catch {}
  }, 200);
};

const readCSVFiles = (files, onRead, onError = null) => {
  for (var i = 0; i < files.length; i++) {
    var reader = new FileReader();
    reader.onload = function (e) {
      const parseResult = Papa.parse(e.target.result);
      onRead(parseResult);
    };
    reader.onerror = onError
      ? onError
      : function (err) {
        console.error(err);
      };
    reader.readAsText(files[i]);
  }
};

const setFooterFloatBottom = () => {
  setTimeout(() => {
    const f = document.getElementById(FOOTER_ID);
    try {
      if (f) {
        f.style.display = 'block';
        f.className = f.className.replace(/\bfixed-bottom\b/g, "");
      }
    } catch {}
  }, 200);
};

const setFooterHelp = (visible) => {
  setTimeout(() => { 
    const fa = document.getElementById('footer-assistance');
    try {
      if (fa) {
        fa.style.display = visible ? 'block' : 'none';
      }
    } catch {}
  }, 200);
};

function getLocalStorageUser() {
  const u = storage.getItem("currentUser");
  return u
    ? JSON.parse(u)
    : null;
}

function hasRoleId(currentUser, typeOfUserRole, contextField = null) {
  const user = currentUser ? currentUser : getLocalStorageUser();
  const userRoles = user
    ? user.roles
    : null;
  return userRoles && _.some(userRoles, ur => ur.typeOfUserRole === typeOfUserRole && (!contextField || parseInt(ur[contextField], 10) > 0));
}

function toIntOrNull(v) {
  return v ? parseInt(v, 10) : null;
}

function hasAnyRoleId(currentUser, roleAndContextList) {
  if (!roleAndContextList || !roleAndContextList.length) return false;
  const user = currentUser ? currentUser : getLocalStorageUser();
  const userRoles = user
    ? user.roles
    : null;
  return userRoles && _.some(roleAndContextList, rc => hasRoleId(user, rc.typeOfUserRole, rc.contextField));
}

const changeDateFormat = (
  date,
  changeDateFunction,
  fieldName = null,
  itemToEditId = null
) => {
  let formattedDate = null;
  if (dateHelpers.isDateValid(date)) {
    const dateString = dateHelpers.parseDatePickerDate(
      date,
      dateHelpers.MDY4
    );
    formattedDate = dateHelpers.getMomentFromString(dateString);
  }
  if (fieldName && itemToEditId) {
    changeDateFunction(formattedDate, fieldName, itemToEditId);
  } else if (fieldName) {
    changeDateFunction(formattedDate, fieldName);
  } else if (itemToEditId) {
    changeDateFunction(formattedDate, itemToEditId);
  } else {
    changeDateFunction(formattedDate);
  }
};

const onDatePickerKeyDown = (
  event,
  changeDateFunction,
  fieldName = null,
  itemToEditId = null
) => {
  if (event.which === 9 || event.which === 13) {
    // tab key or enter key
    const eventAction = event && event.target ? event.target.value : null;
    changeDateFormat(eventAction, changeDateFunction, fieldName, itemToEditId);
  }
};

const toggleListItemChecked = (id, list) => {
  const idx = _.findIndex(list, x => x.id === id);
  let newList = list.slice();
  newList[idx].checked = newList[idx].checked ? false : true;
  return list;
};

const helpers = {
  resolveRoleLabel(role) {
    return constants.ROLE_DICTIONARY[role.TypeOfUserRole];
  },
  isTrueOrFalse(v) {
    return v === true || v === false;
  },
  booleanIcon(value) {
    return <span className={value ? "text-success" : "text-danger"}>
      <FontAwesomeIcon icon={value ? "check" : "times-circle"} />
    </span>;
  },
  readCSVFiles,
  yesNoList: [{value: true, label: "Yes"}, {value: false, label: "No"}],
  PASSWORD_COMPLEXITY: "Passwords must be at least 8 characters long and contain at least 1 uppercase character, at least 1 lowercase character and at least 1 number.",
  ADMIN_ROLE_ID: 1,
  truncateString: (s, length) => {
    if (!s) return s;
    if (s.length > length) {
      return s.substr(0, length) + "...";
    }
    return s;
  },
  userIs: function(user, roleId) {
    return user && user.roles && (_.includes(user.roles, roleId) || _.includes(user.roles, this.SYSTEM_ADMIN_ROLE_ID));
  },
  getCurrentUser: () => {
    const userString = storage.getItem("currentUser");
    return userString
      ? JSON.parse(userString)
      : null;
  },
  browserExportCSVFile: function(csv, fileTitle) {
    var fname = fileTitle || "export.csv";
    var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    if (navigator.msSaveBlob) {
      // IE 10+
      navigator.msSaveBlob(blob, fname);
    } else {
      var link = document.createElement("a");
      if (link.download !== undefined) {
        // Browsers that support HTML5 download attribute
        var url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", fname);
        link.style.visibility = "hidden";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    }
  },
  browserExportFile: function(csv, fileTitle, type) {
    var fname = fileTitle || "export.csv";
    var blob = new Blob([csv], { type: type });
    if (navigator.msSaveBlob) {
      // IE 10+
      navigator.msSaveBlob(blob, fname);
    } else {
      var link = document.createElement("a");
      if (link.download !== undefined) {
        // Browsers that support HTML5 download attribute
        var url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", fname);
        link.style.visibility = "hidden";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    }
  },
  outcomeText(id) {
    switch (id) {
        case constants.TypeOfOutcome.FOLLOWUP:
            return "Follow Up";
        case constants.TypeOfOutcome.PASS:
            return "Pass";
        case constants.TypeOfOutcome.NOT_CLEARED:
            return "Not Cleared";
        case constants.TypeOfOutcome.UNABLE_TO_REACH:
            return "Unable to Reach";
        default:
            return "N/A";
    }
  },
  outcomeCellStyle(id) {
    switch (id) {
        case constants.TypeOfOutcome.FOLLOWUP:
            return "follow-up";
        case constants.TypeOfOutcome.PASS:
            return "pass";
        case constants.TypeOfOutcome.NOT_CLEARED:
            return "not-cleared";
        case constants.TypeOfOutcome.UNABLE_TO_REACH:
            return "unable-to-reach";
        default:
            return "";
    }
  },
  toggleListItemChecked,
  onDatePickerKeyDown,
  toIntOrNull,
  hasRoleId,
  hasAnyRoleId,
  requiredStar() {
    return <span style={{ color: "red" }}>*</span>;
  },
  booleanListEntry: function (b) {
    return b === true
      ? { label: "Yes", value: true }
      : { label: "No", value: false };
  },
  resolveValue: function (obj, id, name) {
    if (obj) return obj;
    if (id === null) return null;
    return { value: id, label: name };
  },
  resolveValues: function (objs) {
    if (objs.length === 0) {
      return [];
    }
    const formattedObjects = [];
    _.forEach(objs, (obj) => {
      if (obj.id === null) return null;
      formattedObjects.push({ value: obj.id, label: obj.name });
    });
    return formattedObjects;
  },
  yesNoOptions: function () {
    return [
      { label: "Yes", value: true },
      { label: "No", value: false },
    ];
  },

  addAnyOption: (list) => {
    let newList = list.slice();
    newList.unshift({label: "(Any)", value: null});
    return newList;
  },

  formatAddress: function (address) {
    const addressArray = [address.ln1, address.ln2, address.ln3, address.ln4];
    const compactList = _.compact(addressArray, (x) => x);

    compactList.push(address.city + ", " + address.state + " " + address.zip);
    return compactList;
  },

  formatPhoneNumber: function (phoneNumberString) {
    phoneNumberString = phoneNumberString.replace(/[^\d]+/g, "");
    if (phoneNumberString.length === 7) {
      phoneNumberString = phoneNumberString.replace(/(\d{3})(\d{4})/, "$1-$2");
    } else if (phoneNumberString.length === 10) {
      phoneNumberString = phoneNumberString.replace(
        /(\d{3})(\d{3})(\d{4})/,
        "($1) $2-$3"
      );
    } else if (phoneNumberString.length === 11) {
      phoneNumberString = phoneNumberString.replace(
        /^(1|)?(\d{3})(\d{3})(\d{4})/,
        "($2) $3-$4"
      );
    }
    return phoneNumberString;
  },

  browserExportCSVFile: function (csv, fileTitle) {
    const fname = fileTitle || "export.csv";
    const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    if (navigator.msSaveBlob) {
      // IE 10+
      navigator.msSaveBlob(blob, fname);
    } else {
      const link = document.createElement("a");
      if (link.download !== undefined) {
        // Browsers that support HTML5 download attribute
        const url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", fname);
        link.style.visibility = "hidden";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    }
  },

  downloadExport: function (data, fileName, mime) {
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, fileName);
    } else {
      const file = new Blob([data], { type: mime });
      var anchor = window.document.createElement("a");
      anchor.download = fileName;
      anchor.href = (window.webkitURL || window.URL).createObjectURL(file);
      anchor.dataset.downloadurl = [mime, anchor.download, anchor.href].join(
        ":"
      );
      anchor.click();
    }
  },

  idNameToValueLabel(list, propName = null) {
    if (!list || !list.length || list.length === 0) return [];
    const valueLabelList = _.map(list, (x) => ({
            label: x.name,
            value: x.id
          }));
    return propName
      ? {
          [propName]: valueLabelList,
        }
      : valueLabelList;
  },

  addLabelValueToList(list, propName = null) {
    if (!list || !list.length || list.length === 0) return [];
    const valueLabelList = _.map(list, (x) => {
      x.label = x.name
      x.value = x.id;
      return x;
    });
    return propName
      ? {
          [propName]: valueLabelList,
        }
      : valueLabelList;
  },

  catchHandler: (e) => console.error(e),

  mustChangePassword: function (user) {
    if (!user) return false;
    return user.mustChangePassword;
  },

  //expecting arrayofKeysToCheck to be array of objects with keys "name" (name of the key on the object) and "label" (what you tell the user is missing)
  //example: [{name: "firstName", label: "first name"}, {name: "email", label: "e-mail address"}]
  isFormInvalid(object, arrayOfKeysToCheck) {
    const warnings = [];
    _.map(arrayOfKeysToCheck, (key) => {
      if (!object[key.name]) {
        warnings.push(key.label);
      }
    });

    if (warnings.length) {
      return "Please provide the following: " + warnings.join(", ");
    } else {
      return false;
    }
  },

  addIsEditingField(list, propName = null) {
    if (!list || !list.length || list.length === 0) {
      return [];
    }
    const isEditingList = _.map(list, (x) => {
      return {
        ...x,
        isEditing: false,
      };
    });
    return propName ? { [propName]: isEditingList } : isEditingList;
  },

  nullableString(s) {
    return s ? s : "";
  },
  emailIsValid: function (email) {
    var emailRe = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return emailRe.test(email);
  },
  isMobile: function (window) {
    const smallerOption =
      window.document &&
      window.document.defaultView &&
      window.document.defaultView.innerWidth
        ? window.document.defaultView.innerWidth
        : window.screen.width;
    return smallerOption <= 680;
  },
  cardHeaderName: function cardHeaderName(cardName, iconName) {
    return (
      <span>
        <FontAwesomeIcon icon={iconName} className="mr-2" />
        {cardName}
      </span>);
  },
  getStateFromLinkParams(props, key = null) {
    let paramValue = props && props.location && props.location.state;
    if (key) {
      paramValue = props.location.state[key];
    }
    return paramValue
      ? paramValue
      : props;
  },
}

export default helpers;
