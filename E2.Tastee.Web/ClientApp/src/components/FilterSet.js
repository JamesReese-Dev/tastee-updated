import React, { useEffect, useState } from "react";
import { Row, Collapse, Button, Col } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import _ from "lodash";

export default function FilterSet(props) {
  const [filterDisplayText, setFilterDisplayText] = useState('')
  const [collapse, setCollapse] = useState((props.open || false))

  useEffect(() => {
    let filterDisplay = ''
    if (!props.filters || !props.filters.length || props.filters.length === 0) {
      return filterDisplay;
    }
    filterDisplay = _.chain(props.filters)
      .map(x => {
        return x.value !== null || x.values || x.startDate || x.endDate
          ? x.filteredDisplay
          : null}
      )
      .filter(x => x !== null && x !== undefined)
      .value()
      .join(" | ");
    setFilterDisplayText(filterDisplay !== '' ? ` - ${filterDisplay}` : '')
  }, [props.filters])

  function toggleFilter() {
    let newCollapse = !collapse;
    setCollapse(newCollapse)
    if (props.toggleFilters) {
      props.toggleFilters(newCollapse);
    }
  }
  const showButtonArea = props.buttonArea;
  const filterDisplayAreaSize = showButtonArea ? (12 - props.buttonAreaColSize) : 12;

  function clearFilterSet() {
    setFilterDisplayText('')
    props.clearFilters()
  }

  return (
    <Row className="m-0 p-0">
      <Col className="m-0 px-3 py-1 pb-3">
        <Row>
          <Col xs={filterDisplayAreaSize}>
            {props.children && (
              <span
                title="Toggle filter display"
                className={
                  props.customFilterClassName
                    ? props.customFilterClassName
                    : "filter"
                }
                onClick={toggleFilter}
              >
                <FontAwesomeIcon icon="filter" /> Filters
              </span>
            )}
            <span className="filter-display" title={filterDisplayText}>
              {filterDisplayText}
            </span>
            {props.filters && props.filters.length !== 0 &&
              (
              <Button
                className="projectDangerOutline ml-4 xs"
                size="sm"
                style={{ marginTop: "-6px", marginBottom: "4px" }}
                onClick={clearFilterSet}
              >
                <FontAwesomeIcon
                  icon="backspace"
                  className="mr-2 ml-0 mt-0 mb-0"
                />{" "}
                Clear
              </Button>
            )}
          </Col>
          {showButtonArea &&
            <Col xs={props.buttonAreaColSize || "2"} className={props.buttonAreaClassName || ""}>
              {props.buttonArea}
            </Col>
          }
        </Row>
        <Collapse isOpen={collapse}>
          <Row
            className={
              props.childrenClassName ? props.childrenClassName : "filter-definition-row"
            }
          >
            <Col xs="12">
              {props.children}
            </Col>
          </Row>
        </Collapse>
      </Col>
    </Row>
  );
}