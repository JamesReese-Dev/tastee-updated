import React, { useContext, Fragment } from "react";
import { Row, Col } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { UserContext, storage } from "../utils";

export default function Footer(props) {
  const userCtx = useContext(UserContext);
  const currentUser = userCtx && userCtx.currentUser ? userCtx.currentUser : null;
  let appVersion = "";
  try {
    appVersion = storage.getItem("app-version");
  } catch {}

  return (
    <div className={`${currentUser ? "footerAuth" : ""} footer text-white mr-0 pr-0`}>
      <Row className="m-0 px-0 pt-2">
        <Col xs="1" />
        <Col xs="5" sm="6" md="7" className="px-2"></Col>
        <Col
          xs="6"
          sm="5"
          md="4"
          className="pl-0 pr-4 text-right align-self-center"
        >
          Tastee &#169; 2021 (v{appVersion})
        </Col>
      </Row>
    </div>
  );
}
