import React, { useState } from "react";
import helpers from "../utils/helpers";
import {
  Container,
  Col,
  Row,
  Card,
  CardBody,
  Alert,
  Button,
  Label,
  Input,
} from "reactstrap";
import api from "../utils/api";
import _ from "lodash";
import { Link, Redirect } from "react-router-dom";
import Logo from "../assets/images/logo.png";

export default function ForgotPassword(props) {
  const submitURL = "Public/ForgotPassword";
  const backToSignInPath = "/login";
  const [authData, setAuthData] = useState({ username: "" });
  const [redirectTo, setRedirectTo] = useState("");
  const [message, setMessage] = useState(null);

  function validate() {
    let warnings = [];
    if (!authData.username) {
      warnings.push("Username is required");
    }
    return warnings;
  }

  function onChange(fieldName, fieldValue) {
    let newData = Object.assign({}, authData);
    newData[fieldName] = fieldValue;
    setAuthData(newData);
  }

  function onSubmit() {
    const warnings = validate();
    if (warnings.length) {
      setMessage({
        text: warnings.join(" "),
        flavor: "alert-danger",
      });
      return;
    } else {
      setMessage(null);
    }
    api
      .post(submitURL, {
        username: authData.username,
      })
      .then((response) => {
        if (response.data.success) {
          setMessage({
            flavor: "alert-success",
            text: "If we have a user with that address, we've just sent a reset link!",
          });
          setTimeout(() => {
            setRedirectTo("/");
          }, 5000);
        } else {
          setMessage({ flavor: "alert-danger", text: response.data.message });
        }
      })
      .catch(helpers.catchHandler);
  }

  if (redirectTo) {
    return <Redirect to={redirectTo} />;
  }
  return (
    <Container fluid>
      <Row className="m-0 p-0 fullHeight">
        <Col className="m-0">
          <Row className="m-0 pt-5 fullHeight">
            <Col className="m-0 p-0">
      <Row className="fullHeight align-items-center">
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
                    <strong>Forgot Password</strong>
                  </h4>
                </Col>
              </Row>
              <Row>
                <Col sm="12" className="text-center">
                  <a href="/">
                    <img id="Logo" src={Logo} alt="logo" />
                  </a>
                </Col>
              </Row>
              {message ? (
                <Row className="mb-2">
                  <Col>
                    <Alert className={message.flavor}>{message.text}</Alert>
                  </Col>
                </Row>
              ) : null}
              <Row className="minHeight225 mt-4">
                <Col xs="12">
                  <div>
                    <Link to={backToSignInPath}>
                      {" "}
                      Wait, I already have an account - take me to log in!
                    </Link>
                  </div>
                  <div>
                    <form>
                      <Row className="mt-2 mb-2">
                        <Col>
                          <Label className="Label">
                            Enter your username and we will send you a reset
                            link.
                          </Label>
                          <Input
                            type="text"
                            value={authData.username}
                            onChange={(e) =>
                              onChange("username", e.target.value)
                            }
                            placeholder="Username"
                            maxLength="250"
                          />
                        </Col>
                      </Row>
                      <Button
                        className="projectPrimary float-right"
                        onClick={onSubmit}
                      >
                        Reset Password
                      </Button>
                    </form>
                  </div>
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
