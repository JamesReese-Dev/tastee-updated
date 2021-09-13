import React, { Fragment, useState, useEffect } from 'react';
import Select from "react-select";
import makeAnimated from "react-select/animated";
import _ from 'lodash';

export default function FilterSelect(props) {
  const [value, setValue] = useState(null);
  const [options, setOptions] = useState([]);

  useEffect(() => {
    setValue(props.value || null);
    setOptions(props.options || []);
  }, [props.value, props.options])

  function onChange(selection) {
    const label = selection === null ? null : selection.label;
    props.onChangeCallback({
      filterName: props.filterName,
      value: selection === null ? null : selection,
      label: label,
      filteredDisplay: selection === null
        ? ''
        : `${props.displayName}: ${label}`
    });
  }

  if (!options) return null;
  return (
    <Fragment>
      {props.displayName ?
        <div className="filter-definition-caption">{props.displayName}</div>
        : null}
      <span style={props.style}>
        <Select
          closeMenuOnSelect={true}
          isMulti={false}
          components={makeAnimated()}
          options={options}
          onChange={onChange}
          isClearable={true}
          value={value}
          onBlurResetsInput={false}
          onSelectResetsInput={false}
          onCloseResetsInput={false}
          classNamePrefix="react-select"
        />
      </span>
    </Fragment>
  );
}
