import React, { useContext, useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { Container, Row, Col, Button } from "reactstrap";
import _ from "lodash";
import { DashboardAdmin } from "./";
import { UserContext, storage, constants, helpers } from "../utils";
import Logo from "../assets/images/logo.png";

export default function Home(props) {
  const userCtx = useContext(UserContext);
  const currentUser = userCtx.currentUser;

  if (currentUser) {
    return <DashboardAdmin {...props} />;
  } else {
    return (
    <Container fluid>
      <Row className="m-0 p-0 fullHeight">
        <Col className="m-0">
          <Row className="m-0 pt-5 fullHeight">
            <Col className="m-0 p-0">
        <Row className="align-items-center fullHeight">
          <Col xs={{ size: 10, offset: 1 }}>
            <Row
              style={{
                backgroundColor: "#FFF",
                borderRadius: "8px",
                padding: "16px 20px",
              }}
            >
              <Col sm="12">
                <Row>
                  <Col sm="12" className="text-center">
                    <h4>
                      <strong>Welcome</strong>
                    </h4>
                  </Col>
                </Row>
                <Row>
                  <Col sm="12" className="text-center">
                    <img id="Logo" src={Logo} alt="logo" />
                  </Col>
                </Row>
                <Row className="minHeight225 align-content-center">
                  <Col xs="12" sm="6" className="text-center px-1">
                    <Button
                      className="projectPrimary mb-2"
                      tag={Link}
                      to="/register"
                      size="lg"
                    >
                      Register
                    </Button>
                  </Col>
                  <Col xs="12" sm="6" className="text-center px-1">
                    <Button
                      className="projectPrimary mb-2"
                      tag={Link}
                      to="/login"
                      size="lg"
                    >
                      Sign In
                    </Button>
                  </Col>
                </Row>
              </Col>
            </Row>
          </Col>
        </Row>
            </Col>
          </Row>
        </Col>
      </Row>
    </Container>
    );
  }
}
