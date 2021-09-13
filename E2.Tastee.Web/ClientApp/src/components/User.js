import React, { Fragment, useState, useContext, useEffect, useRef } from "react";
import { Link, Redirect } from 'react-router-dom';
import { Media } from "react-breakpoints";
import { Col, Button, Row, ButtonGroup, ListGroup, ListGroupItem, Card, CardTitle, CardHeader, CardBody, 
  Badge, Table, FormGroup, Label, Input, Alert } from "reactstrap";
import Select from "react-select";
import NumberFormat from "react-number-format";
import Dropzone from 'react-dropzone';
import DebounceInput from "react-debounce-input";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classnames from "classnames";
import _ from "lodash";
import Switch from "react-switch";
import { api, helpers, UserContext, constants } from "../utils";
import { DesktopContainer } from "../layouts";

const EMPTY_ROLE = {
  typeOfUserRole: null
};
const roleGroupStyles = {
  alignItems: 'center',
  fontSize: 14,
  fontWeight: 'bold',
  textDecoration: 'underline',  
  margin: '2px 0 0 0'
};

const formatRoleGroupLabel = data => (
  <div style={roleGroupStyles}>
    <span>{data.label}</span>
  </div>
);

const defaultNewUser = {
  id: 0,
  firstName: '',
  lastName: '',
  mothersMaidenName: '',
  email: '',
  usersBloodType: '',
  username: '',
  mobilePhoneNumber: '',
  timezone: constants.DEFAULT_TIMEZONE,
  roles: []
};
  
export default function User(props) {
  const BloodTypes = [
    {
      label: "A+",
      value: "A+"
    },
    {
      label: "A-",
      value: "A-"
    },
    {
      label: "B+",
      value: "B+"
    },
    {
      label: "B-",
      value: "B-"
    },
    {
      label: "AB+",
      value: "AB+"
    },
    {
      label: "AB",
      value: "AB-"
    },
    {
      label: "O+",
      value: "O+"
    },
    {
      label: "O-",
      value: "O-"
    },
    {
      label: "Non-Human",
      value: "Non-Human"
    }

  ];

  const userCtx = useContext(UserContext);
  const routeUserId = props?.computedMatch?.params?.id
    ? parseInt(props.computedMatch.params.id, 10)
    : null;
  const [user, setUser] = useState(defaultNewUser);
  const [userRoles, setUserRoles] = useState(user.roles || []);
  const [timezoneList, setTimezoneList] = useState([]);
  const [newRole, setNewRole] = useState("");
  const [message, setMessage] = useState(null);
  const [roleSelectionState, setRoleSelectionState] = useState(null);
  const [redirectTo, setRedirectTo] = useState("");
  const isNewUser = !routeUserId;
  const currentUser = userCtx?.currentUser
      ? userCtx.currentUser
      : null;

  const getUser = () => api.fetch(`UserAdmin/User/${routeUserId}`).then(r => ({
    user: r.data
  }));

  useEffect(() => {
    let apiCalls = [];
    apiCalls.push(api.getTimezones());
    if (routeUserId) 
    {
      apiCalls.push(getUser());
    }
    Promise.all(apiCalls).then(arrayResults => {
      let aggResults = {};
      _.each(arrayResults, x => Object.assign(aggResults, x));
      setTimezoneList(aggResults.timezones ? aggResults.timezones : []);
      const existingUser = aggResults.user ? aggResults.user : null;
      if (existingUser) {
        setUser(existingUser);
      }
    }).catch(error => setMessage({flavor: "alert-danger", text: "There was an error loading user data"}));
  }, [])

  function onChange(fieldName, fieldValue) {
    let newUser = Object.assign({}, user);
    newUser[fieldName] = fieldValue;
    setUser(newUser);
  }

  function onRemoveRole(selectedRole) {
    let newUser = Object.assign({}, user);
    newUser.roles = _.reject(user.roles, r => r.typeOfUserRole === selectedRole.typeOfUserRole 
          && r.tenantId === selectedRole.tenantId);
    setUser(newUser);
  }

  function appendRoleWithContent(roleList, newRole) {
    let builtNewRole = {
      ...EMPTY_ROLE,
      typeOfUserRole: newRole.typeSelection.value
    };
    if (newRole.typeSelection.value === constants.ROLE_IDS.SYSADMIN) {
      roleList.push(builtNewRole);
      return roleList;
    }
    roleList.push(builtNewRole);
    return roleList;
  }

  function freshRoleState(typeSelection) {
    let nextRole = {
      typeSelection: typeSelection
    };
    return nextRole;
  }

  function onAddRole() {
    if (!roleSelectionState?.newRole?.typeSelection) {
      setMessage({flavor: "projectWarning", text: "Select a role before attempting to add it to the list."});
      return;
    }
    if (roleSelectionState?.newRole?.typeSelection.value === constants.ROLE_IDS.SYSADMIN
      && _.some(user.roles, ur => ur.typeOfUserRole === constants.ROLE_IDS.SYSADMIN)) 
    {
      setMessage({flavor: "projectWarning", text: 'The system administrator role has already been given.'});
      return;
    }
    let duplicate = false;
    for(var j=0; j<user.roles.length; j++) {
      const ur = user.roles[j];
      if (ur.typeOfUserRole === roleSelectionState.newRole.typeSelection.value) {
        duplicate = true;
        break;
      }
    }
    if (duplicate) {
      setMessage({flavor: "projectWarning", text: 'This role has already been given.'});
      return;
    }
    let newUser = Object.assign({}, user);
    newUser.roles = appendRoleWithContent(user.roles.slice(), roleSelectionState.newRole);
    setUser(newUser);
    setRoleSelectionState({
      ...roleSelectionState,
      newRole: freshRoleState(roleSelectionState.newRole.typeSelection)
    });
  }

  function onRoleSelectChange(fieldName, selection) {
    let newRoleState = freshRoleState(roleSelectionState?.newRole?.typeSelection);
    newRoleState[fieldName] = selection;
    setRoleSelectionState({
      newRole: newRoleState
    });
  }

  function validate() {
    let warnings = [];
    if (!user.firstName || _.trim(user.firstName).length < 2) {
      warnings.push('Please provide a valid first name.');
    }
    if (!user.lastName || _.trim(user.lastName).length < 2) {
      warnings.push('Please provide a valid last name.');
    }
    if ((!user.password || user.password.length < 8) && user.id === 0) {
      warnings.push('Please provide a password of at least 8 characters.');
    }
    if (!user.email || !helpers.emailIsValid(_.trim(user.email))) {
      warnings.push('Please provide a valid email.');
    }
    if (!user.roles || user.roles.length === 0) {
      warnings.push('Please select at least 1 role for the user.')
    }
    return warnings;
  }

  function onSave() {
    window.scroll(0, 0);
    const warnings = validate();
    if (warnings.length) {
      setMessage({
        text: warnings.join(' '),
        flavor: 'alert-danger'
      });
      return;
    } else {
      setMessage(null);
    }
    
    let payload = Object.assign({}, user);
    payload.firstName = _.trim(payload.firstName);
    payload.lastName = _.trim(payload.lastName);
    payload.email = _.trim(payload.email);
    payload.username = payload.email; // username is always set to email
    payload.mothersMaidenName = _.trim(payload.mothersMaidenName);
    payload.usersBloodType = payload.usersBloodType;
    if (isNewUser) { 
      delete payload.id; 
    }
    api.post('UserAdmin/SaveUser', payload).then(response => {
      if (response.data.success) {
        setUser({...user, id: response.data.message.id})
        setRedirectTo("/users/admin");
      } else {
        setMessage({text: response.data.message, flavor: "alert-danger"});
      }
    }).catch(helpers.catchHandler);
  }

  const newSelectedRole = roleSelectionState?.newRole?.typeSelection
    ? _.find(constants.roles, r => r.value === roleSelectionState.newRole.typeSelection.value)
    : null;
  let newSelectedRoleContextDescription = null;
  const fullName = user?.lastName ? `${user.firstName} ${user.lastName}` : 'User';
  if (!currentUser) return null;
  if (redirectTo) {
    return (<Redirect to="/admin/users" />)
  }
  return (
  <DesktopContainer screenName={`User Details`}>
    {message
      ? (<Alert className={message.flavor}>{message.text}</Alert>)
      : null
    }
    <Row className="mb-3">
      <Col xs="12">
        <Card>
          <CardBody>
            <Row className="mb-1">
                <Col xs="3">First Name {helpers.requiredStar()}
                <DebounceInput
                  type="text"
                  name="firstName"
                  id="firstName"
                  maxLength="30"
                  onChange={(event) => onChange(event.target.name, event.target.value)}
                  value={user.firstName || ""}
                  className="form-control"
                  debounceTimeout={300}
                />
              </Col>
              <Col xs="3">Last Name {helpers.requiredStar()}
                <DebounceInput
                  type="text"
                  name="lastName"
                  id="lastName"
                  maxLength="30"
                  onChange={(event) => onChange(event.target.name, event.target.value)}
                  value={user.lastName || ""}
                  className="form-control"
                  debounceTimeout={300}
                />
              </Col>
              <Col xs="3">Email {helpers.requiredStar()}
                <DebounceInput
                  type="text" 
                  name="email"
                  id="email"
                  maxLength="50"
                  onChange={(event) => onChange(event.target.name, event.target.value)}
                  value={user.email || ""}
                  className="form-control"
                  debounceTimeout={300}
                />
              </Col>
            </Row>
            <Row className="mb-1">
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
              <Col xs="3">Timezone
                <Select
                  options={timezoneList}
                  value={_.find(timezoneList, x => x.value === user.timezone)}
                  onChange={(option) => onChange('timezone', option.value)}
                />
              </Col>
                <Col xs="3"> Blood Type
                  <Select
                    id="bloodType"
                    name="bloodType"
                    default={user.bloodType ? user.bloodType : null}
                    options={BloodTypes}
                    onChange={(option) => onChange('userBloodType', option.value)} />
                </Col>
                <Col xs="3">Mothers Maiden Name
                  <DebounceInput
                    type="text"
                    name="mothersMaidenName"
                    id="mothersMaidenName"
                    maxlength="80"
                    value={user.mothersMaidenName || ""}
                    className="form-control"
                    onChange={(event) => onChange(event.target.name, event.target.value)}
                  />
                </Col>
              {isNewUser
                ? (<Col xs="6">Password {helpers.requiredStar()}
                    <span className="float-right" style={{backgroundColor: "#F0F0F0", maxWidth: "70%", padding: "7px"}}>{constants.PASSWORD_COMPLEXITY_DESCRIPTION}</span>
                    <DebounceInput
                      type="password"
                      name="password"
                      id="password"
                      maxLength="30"
                      style={{width: "180px"}}
                      onChange={(event) => onChange(event.target.name, event.target.value)}
                      value={user.password || ""}
                      className="form-control"
                      debounceTimeout={300}
                    />
                  </Col>)
                : null
              }
            </Row>
          </CardBody>
        </Card>
      </Col>
    </Row>
    <Row>
      <Col xs="12">
        <Card>
          <CardHeader>
            <Row>
              <Col>
                <h5>Adjust {fullName} Role Membership</h5>
              </Col>
            </Row>
          </CardHeader>
          <CardBody>
            <Row className="pt-3">
              <Col xs="2">
                <Label className="mb-0">Roles</Label>
                <Select
                  options={constants.ALL_ROLES}
                  formatGroupLabel={formatRoleGroupLabel}
                  value={roleSelectionState?.newRole?.typeSelection}
                  onChange={(option) => onRoleSelectChange('typeSelection', option)}
                />
                {roleSelectionState && roleSelectionState.newRole
                  ? (<div style={{margin: "4px", padding: "4px", backgroundColor: "#f2f5ff", borderRadius: "4px"}}>
                      <b>{roleSelectionState.newRole.typeSelection.label}</b>{newSelectedRoleContextDescription}
                    </div>)
                  : null
                }                              
              </Col>
              <Col className="pt-4">
                <Button className="projectSuccess float-right" onClick={onAddRole} title="Add this role">
                  <FontAwesomeIcon icon="plus" />
                </Button>
              </Col>
            </Row>
            <Row className="pt-3">
              <Col>
                <Table size="sm">
                  <thead>
                    <tr>
                      <th>Role Name</th>
                      <th>Context</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                  {_.sortBy(user.roles, r => constants.ROLE_DICTIONARY[r.typeOfUserRole]).map((r, index) => (
                    <tr key={`${r.typeOfUserRole}-${index}`}
                    >
                      <td>{constants.ROLE_DICTIONARY[r.typeOfUserRole]}</td>
                      <td>{r.label}</td>
                      <td>
                        <Button onClick={() => onRemoveRole(r)} className="projectDanger" size="sm" title="Remove">
                          <FontAwesomeIcon icon="times-circle" />
                        </Button>
                      </td>
                    </tr>
                    ))}
                  </tbody>
                </Table>
              </Col>
            </Row>
          </CardBody>
        </Card>
      </Col>
    </Row>
    <Row className="my-4">
      <Col xs="12">
        <Row>
          <Col className="text-right">
            <ButtonGroup>
              <Button
                color="secondary"
                tag={Link}
                to="/admin/users"
              >
                Cancel
              </Button>
              <Button
                className="projectPrimary"
                onClick={onSave}
              >
                Save
              </Button>
            </ButtonGroup>
          </Col>
        </Row>
      </Col>
    </Row>
  </DesktopContainer>
  );
}
