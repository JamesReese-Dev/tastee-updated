import React from "react";
import Picker from "react-mobile-picker";
import moment from "moment";
import _ from "lodash";

// TODO: finish scrollwheel date picker

export default function DateSelector(props) {
  const now = moment();

  const valueGroups = {
    month: props.month || now.format("MM"),
    day: props.day || now.format("D"),
    year: props.year || now.format("YYYY"),
  };

  const optionGroups = {
    month: _.range(1, 12),
    day: _.range(1, 31),
    year: _.range(1900, 2050),
  };

  return (
    <div className="picker-inline-container">
      <Picker
        optionGroups={optionGroups}
        valueGroups={valueGroups}
        onChange={props.onChange}
        height={96}
      />
    </div>
  );
}
