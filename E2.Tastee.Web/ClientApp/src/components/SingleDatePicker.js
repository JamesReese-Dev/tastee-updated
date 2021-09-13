import React, { useState, useEffect } from "react";
import moment from "moment";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { helpers, dates } from "../utils";

export default function SingleDatePicker(props) {
  const [selectedDate, setSelectedDate] = useState(null);

  useEffect(() => {
    if (props.selected) {
      let parsedDate = props.selected.isValid && props.selected.isValid() ? props.selected.toDate() : moment(props.selected).toDate();
      if (parsedDate) {
        setSelectedDate(parsedDate);
      }
    }
  }, [props.selected]);

  function changeDateSelected(dateChange) {
    const dateMoment = dateChange ? moment(dateChange).toDate() : null;
    setSelectedDate(dateMoment);
    props.onChangeCallback(dateMoment, props.fieldName);
  }

  return (
    <DatePicker
      selected={selectedDate || ""}
      onChange={changeDateSelected}
      className="form-control"
      onKeyDown={(event) =>
        helpers.onDatePickerKeyDown(event, changeDateSelected)
      }
      minDate={props.minDate ? props.minDate : null}
    />
  );
}