import React, { useState } from "react";
import { Link, Redirect } from "react-router-dom";
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
import helpers from "../utils/helpers";
import api from "../utils/api";
import _ from "lodash";
import Logo from "../assets/images/logo.png";

export default function ResetPassword(props) {
  const submitURL =
    _.startsWith(props.location.pathname, "/participant")
      ? "Public/Participant/SetPassword"
      : "Public/SetPassword";
    const backToSignInPath =
      _.startsWith(props.location.pathname, "/participant")
        ? "/signin"
        : "/login";
  const [authData, setAuthData] = useState({
    password: "",
    passwordAgain: "",
    resetToken: props?.computedMatch?.params?.resetToken
      ? props.computedMatch.params.resetToken
      : null,
  });
  const [redirectTo, setRedirectTo] = useState("");
  const [message, setMessage] = useState(null);

  function onChange(fieldName, fieldValue) {
    let newData = Object.assign({}, authData);
    newData[fieldName] = fieldValue;
    setAuthData(newData);
  }

  function validate() {
    let warnings = [];
    if (!authData.password || authData.password.length < 8) {
      warnings.push(
        "Password is required and must be at least 8 characters long."
      );
    }
    if (authData.password !== authData.passwordAgain) {
      warnings.push("The password and password confirmation do not match.");
    }
    return warnings;
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
        token: authData.resetToken,
        password: authData.password,
      })
      .then((response) => {
        if (response.data.success) {
          setMessage({
            flavor: "alert-success",
            text: "You have successfully reset your password. We're now redirecting you to the home screen.",
          });
          setTimeout(() => {
            setRedirectTo(backToSignInPath);
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
                    <strong>Reset Password</strong>
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
                      Wait, I remember my account info - take me to log in!
                    </Link>
                  </div>
                  <div>
                    <form>
                      <Row className="mt-2 mb-2">
                        <Col>
                          <Label className="label"> New Password </Label>
                          <Input
                            type="password"
                            value={authData.password}
                            onChange={(e) => onChange("password", e.target.value)}
                            placeholder="Password"
                          />
                        </Col>
                      </Row>
                      <Row className="mb-4">
                        <Col>
                          <Label className="label"> Confirm Password </Label>
                          <Input
                            type="password"
                            value={authData.passwordAgain}
                            onChange={(e) =>
                              onChange("passwordAgain", e.target.value)
                            }
                            placeholder="Re-Type Password"
                          />
                        </Col>
                      </Row>
                      <Button
                        className="projectPrimary float-right"
                        onClick={onSubmit}
                      >
                        Confirm Password Reset
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
