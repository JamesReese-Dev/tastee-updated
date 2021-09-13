import React, { Fragment, useState, useEffect } from 'react';
import DateTimePicker from 'react-datetime-picker'
import { dateHelpers } from "../utils";
// import moment from "moment";

export default function FilterDateTime(props) {
  const [value, setValue] = useState(false);

  useEffect(() => {
    setValue(props.value)
  }, [props.value])

  function onChange(value) {
    const filteredDisplay = value
        ? `${props.displayName} ${dateHelpers.dateTimeFormat(value, dateHelpers.MDYHMA)}`
        : '';
    props.onChangeCallback({
      filterName: props.filterName,
      value: value,
      filteredDisplay,
    });
    setValue(value);
  }

  return (
  <Fragment>
    {props.hidePrompt
      ? null
      : (<div className='filter-definition-caption' style={{display: 'block'}}>{props.displayName}</div>)
    }
    <DateTimePicker
      className="bg-light"
      format="MM/dd/yy hh:mm a"
      onChange={onChange}
      value={value}
    />
  </Fragment>);
}
