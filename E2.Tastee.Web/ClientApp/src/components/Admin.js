import React, { Fragment, useState, useContext } from "react";
import { Link } from "react-router-dom";
import classnames from "classnames";
import { Media } from "react-breakpoints";
import {
  Col,
  Button,
  Row,
  Table,
  Nav,
  NavItem,
  NavLink,
} from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import _ from "lodash";
import { constants, UserContext } from "../utils";
import { LinkPanel } from "../components";
import { DesktopContainer } from "../layouts";
import { IoLibraryOutline } from "react-icons/io5";

export default function Admin(props) {
  const userCtx = useContext(UserContext);
  return (
    <DesktopContainer screenName={"Administration"}>
      <Row className="mt-2 mb-2">
        <LinkPanel
          key='link-users'
          colSize="4"
          iconKey="user"
          title="Users"
          routePath="/admin/users"
        />
        {_.map(constants.REFERENCE_DATA_URL_LIST, (dt) => (
          <LinkPanel
            key={`link-${dt.pageTitle}`}
            colSize="4"
            iconKey="tags"
            title={dt.pageTitle}
            routePath={dt.reactPath}
            data={dt}
          />
        ))}
      </Row>
    </DesktopContainer>
  );
}
