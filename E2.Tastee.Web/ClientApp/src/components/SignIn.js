import React, { useState, useContext } from "react";
import { Alert, Form, Row, Col, Container, Button } from "reactstrap";
import _ from "lodash";
import classnames from "classnames";
import { Link, Redirect } from "react-router-dom";
import Logo from "../assets/images/logo.png";
import { api, helpers, UserContext, storage } from "../utils";

export default function SignIn(props) {
  const submitURL = "Public/Authenticate";
  const forgotPasswordPath = "/forgot_password";
  const redirectPath = "/";
  const userCtx = useContext(UserContext);
  const [username, setUserName] = useState(
    storage.getItem("lastUsername") || ""
  );
  const [password, setPassword] = useState("");
  const [redirectTo, setRedirectTo] = useState(null);
  const [message, setMessage] = useState(null);

  function handleSubmit(e) {
    e.preventDefault();

    api
      .post(submitURL, {
        Username: username,
        Password: password,
      })
      .then((response) => {
        if (response.data.success) {
          setMessage({ flavor: "alert-success", text: "Log-In Successful!" });
          userCtx.signIn(response.data.user, response.data.user.token);
          setTimeout(() => {
            setRedirectTo(redirectPath);
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
  const canSubmit =
    username && username.length > 2 && password && password.length > 7;
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
                    <strong>Sign In</strong>
                  </h4>
                </Col>
              </Row>
              {message ? (
                <Row className="mb-2">
                  <Col>
                    <Alert className={message.flavor}>{message.text}</Alert>
                  </Col>
                </Row>
              ) : null}
              <Row>
                <Col sm="12" className="text-center">
                  <a href="/">
                    <img id="Logo" src={Logo} alt="logo" />
                  </a>
                </Col>
              </Row>
              <Row className="minHeight225">
                <Col xs="12">
                  <Form id="sign_in_form" onSubmit={handleSubmit}>
                    <div className="form-group">
                      <label>Username</label>
                      <input
                        className="form-control"
                        autoFocus
                        type="text"
                        style={{ marginTop: "0" }}
                        value={username}
                        onChange={(e) => setUserName(e.target.value)}
                        placeholder="Enter username"
                      />
                    </div>
                    <div className="form-group">
                      <label>Password</label>
                      <input
                        className="form-control"
                        type="password"
                        style={{ marginTop: "0" }}
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Enter password"
                      />
                    </div>
                    <div style={{ float: "left" }}>
                      <Button
                        type="submit"
                        disabled={!canSubmit}
                        className={classnames(
                          {
                            projectPrimary: canSubmit,
                            projectSecondary: !canSubmit,
                          },
                          "float-right"
                        )}
                        style={{ marginTop: "0" }}
                      >
                        Sign In
                      </Button>
                    </div>
                    <div style={{ float: "right" }} className="text-left">
                      <Link
                        to={{
                          pathname: forgotPasswordPath,
                          state: { email: username },
                          push: true,
                        }}
                      >
                        Forgot Password
                      </Link>
                    </div>
                  </Form>
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
