import React, { useState, useContext, useEffect, Fragment } from "react";
import classnames from "classnames";
import { Redirect } from "react-router-dom";
import { Media } from "react-breakpoints";
import { Card, CardBody, Col, Button, Row, Alert as Alert2, Nav, NavItem, NavLink, TabContent, TabPane } from "reactstrap";
import NumberFormat from "react-number-format";
import DebounceInput from "react-debounce-input";
import Select from "react-select";
import _ from "lodash";
import { api, helpers, UserContext, storage } from "../utils";
import { CollapsibleCardSection } from "../components";
import { DesktopContainer } from "../layouts";

const TABS = {
  DETAILS: "Profile Details",
  PASSWORD: "Credentials"
};

export default function UserProfile(props) {
  const userCtx = useContext(UserContext);
  const [user, setUser] = useState(userCtx.currentUser);
  const [activeTab, setActiveTab] = useState(TABS.DETAILS);
  const [timezoneList, setTimezoneList] = useState([]);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    api.getTimezones().then(r => setTimezoneList(r.timezones));
  })

  useEffect(() => {
    setUser(userCtx.currentUser);
  }, [userCtx.currentUser]);

  function onChange(fieldName, fieldValue) {
    let newUser = Object.assign({}, user);
    newUser[fieldName] = fieldValue;
    setUser(newUser);
  }

  function onSubmit() {
    let warnings = [];
    if (!user.firstName || _.trim(user.firstName).length < 2) {
      warnings.push('Please provide a valid first name.')
    }
    if (!user.lastName || _.trim(user.lastName).length < 2) {
      warnings.push('Please provide a valid last name.')
    }
    if (!user.email || !helpers.emailIsValid(user.email)) {
      warnings.push('Please provide a valid email.')
    }
    if (warnings.length) {
      setMessage({
        text: warnings.join(' '),
        flavor: 'alert-danger'
      });
      return;
    } else {
      setMessage(null);
    }
    api.post('Public/SaveProfile', user).then(response => {
      if (response.data.success) {
        setMessage({ text: "Your profile has been updated successfully!", flavor: "alert-success" });
        setTimeout(() => userCtx.signIn(user), 3000);
      } else {
        setMessage({ text: response.data.message, flavor: "alert-danger" });
      }
    }).catch(helpers.catchHandler);
  }

  function onUpdatePassword() {
    let warnings = [];
    if (!user.newPassword || _.trim(user.newPassword).length < 8) {
      warnings.push('Please provide a valid, new password of at least 8 characters.')
    }
    if (!user.oldPassword || _.trim(user.oldPassword).length < 8) {
      warnings.push('Please provide a valid, existing password of at least 8 characters.')
    }
    if (!user.newPasswordAgain || _.trim(user.newPasswordAgain).length < 8) {
      warnings.push('Please provide your valid, new password a second time.')
    }
    if (user.newPassword !== user.newPasswordAgain) {
      warnings.push('Your new password and password confirmation do not match.')
    }
    if (user.newPassword === user.oldPassword) {
      warnings.push('Your new password and old password must be different.')
    }
    if (warnings.length) {
      setMessage({
        text: warnings.join(' '),
        flavor: 'alert-danger'
      });
      return;
    } else {
      setMessage(null);
    }
    api.post('Public/UpdatePassword', { password: user.newPassword, oldPassword: user.oldPassword }).then(response => {
      if (response.data.success) {
        setMessage({ text: "Your password has been updated successfully!", flavor: "alert-success" });
        setUser({
          ...user,
          oldPassword: "", newPassword: "", newPasswordAgain: ""
        });
      } else {
        setMessage({ text: response.data.message, flavor: "alert-danger" });
      }
    }).catch(helpers.catchHandler);
  }

  const isImpersonating = storage.getItem("adminToken") ? true : false;
  if (!user) return null;
  return (
    <Fragment>
      {message
        ? (<Alert2 className={message.flavor}>{message.text}</Alert2>)
        : null
      }
      <Row>
        <Col className="mt-3 ml-3">
          <Nav tabs>
            <NavItem className="pointer">
              <NavLink
                className={classnames(
                  {
                    active: activeTab === TABS.DETAILS,
                    "btn-light": activeTab !== TABS.DETAILS
                  },
                  "btn mx-1"
                )}
                onClick={() => setActiveTab(TABS.DETAILS)}
              >
                {TABS.DETAILS}
              </NavLink>
            </NavItem>
            <NavItem className="pointer">
              <NavLink
                className={classnames(
                  {
                    active: activeTab === TABS.PASSWORD,
                    "btn-light": activeTab !== TABS.PASSWORD
                  },
                  "btn mx-1"
                )}
                onClick={() => setActiveTab(TABS.PASSWORD)}
              >
                {TABS.PASSWORD}
              </NavLink>
            </NavItem>
          </Nav>
        </Col>
      </Row>
      <Row>
        <Col className="ml-3 pl-0">
          <TabContent activeTab={activeTab}>
            <TabPane tabId={TABS.DETAILS}>
              <Row className="mb-2 align-items-end">
                <Col xs="12" md="9">
                  <Card>
                    <CardBody>
                      <Row className="mb-1">
                        <Col xs="3">
                          First Name {helpers.requiredStar()}
                          <DebounceInput
                            type="text"
                            name="firstName"
                            id="firstName"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.firstName || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                        <Col xs="3">
                          Last Name {helpers.requiredStar()}
                          <DebounceInput
                            type="text"
                            name="lastName"
                            id="lastName"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.lastName || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                        <Col xs="3">
                          Email {helpers.requiredStar()}
                          <DebounceInput
                            type="text"
                            name="email"
                            id="email"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.email || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                        <Col xs="3">Mobile Phone #
                          <NumberFormat
                            className="form-control"
                            format="(###) ###-####"
                            mask="_"
                            name="mobilePhoneNumber"
                            id="mobilePhoneNumber"
                            maxLength="15"
                            onValueChange={values => {
                              // eslint-disable-next-line no-unused-vars
                              const { formattedValue, value } = values;
                              onChange("mobilePhoneNumber", value);
                            }}
                            value={user.mobilePhoneNumber || ""}
                          />
                        </Col>
                      </Row>
                      <Row className="mb-1">
                        <Col xs="3">
                          Timezone<br/>
                          <Select
                            options={timezoneList}
                            value={_.find(timezoneList, x => x.value === user.timezone)}
                            onChange={(option) => onChange('timezone', option.value)}
                          />
                        </Col>
                        <Col xs="9" className="text-right">
                          <Button
                            className="projectPrimary mt-3"
                            onClick={() => onSubmit()}
                          >
                            Save
                          </Button>
                        </Col>
                      </Row>
                    </CardBody>
                  </Card>
                </Col>
              </Row>
            </TabPane>
            <TabPane tabId={TABS.PASSWORD}>
              <Row className="mb-3">
                <Col xs="4">
                  <Card>
                    <CardBody>
                      <Row className="mb-1">
                        <Col xs="12">
                          <div className="my-1">Old Password {helpers.requiredStar()}</div>
                          <DebounceInput
                            type="password"
                            name="oldPassword"
                            id="oldPassword"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.oldPassword || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                      </Row>
                      <Row className="mb-1">
                        <Col xs="12">
                          <div className="my-1">New Password {helpers.requiredStar()}</div>
                          <DebounceInput
                            type="password"
                            name="newPassword"
                            id="newPassword"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.newPassword || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                      </Row>
                      <Row className="mb-1">
                        <Col xs="12">
                          <div className="my-1">Retype New Password {helpers.requiredStar()}</div>
                          <DebounceInput
                            type="password"
                            name="newPasswordAgain"
                            id="newPasswordAgain"
                            onChange={(event) => onChange(event.target.name, event.target.value)}
                            value={user.newPasswordAgain || ""}
                            className="form-control"
                            debounceTimeout={300}
                          />
                        </Col>
                      </Row>
                      <Row>
                        <Col xs="12" className="text-right">
                          <Button
                            className="projectPrimary"
                            onClick={() => onUpdatePassword()}
                            size="sm"
                          >
                            Update Password
                          </Button>
                        </Col>
                      </Row>
                    </CardBody>
                  </Card>
                </Col>
                {isImpersonating
                  ? (<Col xs="4">
                    <Button
                      color="dark"
                      className="btn-lg mt-4"
                      onClick={() => userCtx.clearImpersonation()}
                    >
                      Stop Impersonating {user.firstName}  {user.lastName}
                      </Button>
                  </Col>)
                  : null
                }
              </Row>
            </TabPane>
          </TabContent>
        </Col>
      </Row>
    </Fragment>);
}
