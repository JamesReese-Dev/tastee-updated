import React, { Fragment, useState, useEffect } from "react";
import { Media } from "react-breakpoints";
import { Row, Col } from "reactstrap";
import _ from "lodash";
import { api, UserContext, filterHelpers } from "../utils";
import {
  FilterableScreen,
  StatCard
} from "./";
import { DesktopContainer } from "../layouts";

export default function DashboardAdmin(props) {
  // const userCtx = useContext(UserContext);
  const [filters, setFilters] = useState([]);

  useEffect(() => {
    loadDashboardData();
  }, []);

  function loadDashboardData() {
    api.post(`admin/GetDashboardData`)
      .then(response => {
        if (response && response.data) {
          // setActiveUsers(response.data.userList);
        }
      });
  }

  function onFilterChange(changedFilter) {
    const filterChanges = filterHelpers.getFilters(filters, changedFilter);
    setFilters(filterChanges);
  }

  function currentFilterValue(name) {
    const filterElement = _.find(filters, (f) => f.filterName === name);
    if (filterElement) {
      if (filterElement.value) {
        return filterElement.value;
      }
      return filterElement.values;
    }
    return "";
  }

  return (
    <DesktopContainer screenName={"Admin Dashboard"}>
      <FilterableScreen
        filters={
          <Fragment>
          (to do)
          </Fragment>
        }
        pager={null}
      >
        <Row>
          <Col xs="12" md="9">
            <Row className="p-0 m-0 mb-2">
              <StatCard
                cardName={"Users"}
                count={0}
                link={`/admin/users`}
                xs="6"
              /> 
            </Row>
          </Col>
        </Row>
      </FilterableScreen>
    </DesktopContainer>);
}
