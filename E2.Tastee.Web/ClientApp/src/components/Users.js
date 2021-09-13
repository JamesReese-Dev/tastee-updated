import React, { Fragment, useState, useContext, useEffect} from "react";
import { Link } from "react-router-dom";
import Alert from "react-s-alert";
import { confirmAlert } from "react-confirm-alert";
// import DebounceInput from "react-debounce-input";
import { Col, Button, Row, Table, ButtonGroup, Modal, ModalHeader, ModalBody, Card, CardBody, Label, Input, ModalFooter, Alert as Alert2 } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import _ from "lodash";
import classnames from "classnames";
import { api, helpers, UserContext, filterHelpers, constants } from "../utils";
import {
  FilterText,
  FilterableScreen,
  FilterSet,
  FilterSwitch,
  // FilterSelect,
  FilterMultiSelect,
  Pager
} from "../components";
import { DesktopContainer } from "../layouts";

const resolveRoleListDescription = (roles) => {
  return _.map(roles, r => constants.ROLE_DICTIONARY[r.typeOfUserRole]).join(', ');
};

export default function Users(props) {
  const userCtx = useContext(UserContext);
  const [filters, setFilters] = useState([{filterName: "ActiveOnly", value: true, filteredDisplay: "Active Only"}]);
  const [paginatedList, setPaginatedList] = useState({list: [], pageNumber: 1, totalPages: 1});
  const [sortField, setSortField] = useState(null);
  const [sortDirection, setSortDirection] = useState(null);
  const [message, setMessage] = useState(null);
  // const [tenants, setTenants] = useState([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [textPhoneNumber, setTextPhoneNumber] = useState(null);
  const [textFullName, setTextFullName] = useState(null);
  const [textMothersMaidenName, setTextMothersMaidenName] = useState(null);
  const [bloodType, setBloodType] = useState(null);
  const [textBody, setTextBody] = useState("");
  const [toggleSelectAll, setToggleSelectAll] = useState(false);
  const [selectedUserIds, setSelectedUserIds] = useState([]);
  const [massNotifyPrompt, setMassNotifyPrompt] = useState('');

  // useEffect(() => {
  //   let apiCalls = [];
  //   apiCalls.push(api.getTenants());
  //   Promise.all(apiCalls).then(arrayResults => {
  //     let aggResults = {};
  //     _.each(arrayResults, x => Object.assign(aggResults, x));
  //     setTenants(aggResults.tenants);
  //   }).catch(error => setMessage({flavor: "alert-danger", text: "There was an error loading user data"}));
  // }, [])

  useEffect(() => {
    refreshData();
  }, [filters, pageNumber])

  function createPayload() {
    let payload = {
      sortField: sortField,
      sortDirection: sortDirection,
      pageNumber: pageNumber
    };
    _.each(filters, filter => payload[filter.filterName] = filter.value);
    return payload;
  }

  function refreshData() {
    api.post('UserAdmin/List', createPayload()).then((response) => {
      setSelectedUserIds([]);
      setPaginatedList(response.data);
    }).catch(helpers.catchHandler);
  }

  function onFilterChange(changedFilter) {
    const filterChanges = filterHelpers.getFilters(filters, changedFilter);
    setFilters(filterChanges);
  }

  function onImpersonate(u) {
    api.post('UserAdmin/Impersonate', {username: u.username}).then((response) => {
      if (response.data.success) {
        userCtx.impersonate(response.data.user, response.data.user.token);
      } else {
        setMessage({ flavor: "danger", text: response.data.message });
      }
    }).catch(helpers.catchHandler);
  }

  function onUnlockUser(selected) {
    api.post(`UserAdmin/UnlockUser/${selected.id}`).then(response => {
      if (response.data.success) {
        refreshData();
      } else {
        setMessage({ flavor: "danger", text: response.data.message });
      }
    }).catch(helpers.catchHandler);
  }

  function onToggleMustChangePassword(selected) {
    api.post(`UserAdmin/ToggleMustChangePassword/${selected.id}`).then(response => {
      if (response.data.success) {
        refreshData();
      } else {
        setMessage({ flavor: "danger", text: response.data.message });
      }
    }).catch(helpers.catchHandler);
  }

  function onToggleActive(selected) {
    api.post(`UserAdmin/ToggleUserActive/${selected.id}`).then(response => {
      if (response.data.success) {
        refreshData();
      } else {
        setMessage({ flavor: "danger", text: response.data.message });
      }
    }).catch(helpers.catchHandler);
  }

  function onDelete(selected) {
    confirmAlert({
      title: "Permanently Delete User",
      message: `This will permanently delete ${selected.firstName} ${selected.lastName}. Are you sure?`,
      buttons: [
        {
          label: "Ok",
          onClick: () => {
            api.post(`UserAdmin/DeleteUser/${selected.id}`).then(response => {
              if (response.data.success) {
                refreshData();
              } else {
                setMessage({ flavor: "danger", text: response.data.message });
              }
            }).catch(helpers.catchHandler);
          }
        },
        {
          label: "Cancel"
        }
      ]
    });
  }

  function promptText(fullName, phoneNumber) {
    setTextPhoneNumber(phoneNumber);
    setTextFullName(fullName);
    setTextMothersMaidenName(textMothersMaidenName);
    setTextBody("");
    setBloodType(bloodType);
  }

  function sendText() {
    api.post("Sms/SendUserText", {
      phoneNumber: textPhoneNumber,
      textBody: textBody
    }).then(response => {
      if (response && response.data) {
        Alert.success(`${textFullName} has been sent your text!`);
        setTextPhoneNumber(null);
        setTextFullName(null);
        setTextMothersMaidenName(null);
        setTextBody("");
      } else {
        Alert.error("There was a failure sending");
      }
    }).catch(() => Alert.error("There was a failure sending"));
  }

  function onSendCustomNotification() {
    // const phoneNumbers = _.chain(paginatedList.list)
    //   .filter(x => selectedUserIds.includes(x.id))
    //   .map(x => x.mobilePhoneNumber)
    //   .value();
    api.post("Admin/NotifyUsers", {
      userIds: selectedUserIds,
      textBody: textBody
    }).then(response => {
      if (response.data.success) {
        Alert.success(response.data.message);
        setMassNotifyPrompt("");
        setTextBody("");
      } else {
        Alert.error(response.data.message);
      }
    }).catch(() => Alert.error("There was a failure sending"));
  }

  function exportUserXLSX() {
    api.postGetBlob("/UserAdmin/export", createPayload()).then(r => {
      if (r.data) {
        const XLSX_FILENAME = "export.xlsx";
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(r.data, XLSX_FILENAME);
        }
        else {
          const file = new Blob([r.data], { type: constants.XLSX_MIME });
          var anchor = window.document.createElement('a');
          anchor.download = XLSX_FILENAME;
          anchor.href = (window.webkitURL || window.URL).createObjectURL(file);
          anchor.dataset.downloadurl = [constants.XLSX_MIME, anchor.download, anchor.href].join(':');
          anchor.click();
        }
      } else {
        throw new Error("Error downloading report");
      }
    }).catch(error => Alert.error(error));
  }

  function onToggleSelectAll(on) {
    console.log('toggle all ' + on)
    setToggleSelectAll(on);
    setSelectedUserIds(on ? _.map(paginatedList.list, x => x.id) : []);
  } 

  return (
  <DesktopContainer screenName={"Users"}>
    <FilterableScreen
      filters={
        <Row>
          <Col xs="10">
            <FilterSet
              filters={filters}
              clearFilters={() => setFilters([])}
            >
              <Row>
                <Col xs="3">
                  <FilterText
                    filterName="Name"
                    displayName="Name"
                    value={filterHelpers.currentFilterValue(filters, "Name")}
                    onChangeCallback={(e) => onFilterChange(e)}
                  />
                </Col>
                  <Col xs="2">
                    <FilterText
                      filterName="MothersMaidenName"
                      displayName="Mothers Maiden Name"
                      value={filterHelpers.currentFilterValue(filters, "MothersMaidenName")}
                      onChangeCallback={(e) => onFilterChange(e)}
                    />
                  </Col>
                  <Col xs="3">
                  <FilterSwitch
                    filterName="ActiveOnly"
                    displayName="Active Only"
                    value={filterHelpers.currentFilterValue(filters, "ActiveOnly")}
                    onChangeCallback={(e) => onFilterChange(e)}
                  />
                </Col>
                <Col xs="3">
                  <FilterMultiSelect
                    filterName="MemberOfRoles"
                    displayName="Roles"
                    values={filterHelpers.currentFilterValue(filters, 'MemberOfRoles')}
                    onChangeCallback={(e) => onFilterChange(e)}
                    options={constants.ALL_ROLES}
                  />
                </Col>
                  <Col xs="3">
                    <FilterMultiSelect
                      filterName="bloodType"
                      displayName="Blood Type"
                      values={filterHelpers.currentFilterValue(filters, 'MemberOfRoles')}
                      onChangeCallback={(e) => onFilterChange(e)}
                      options={constants.ALL_ROLES}
                    />
                  </Col>
                {/*
                <Col xs="3">
                  <FilterSelect
                    filterName="TenantId"
                    displayName="Tenant"
                    options={helpers.addAnyOption(tenants)}
                    value={filterHelpers.currentFilterValue(filters, 'TenantId')}
                    onChangeCallback={(e) => onFilterChange(e)}
                    clearValue={() => onFilterChange({filterName: 'TenantId', value: null})}
                  />
                </Col>
              */}
              </Row>
            </FilterSet>
          </Col>
          <Col xs="2" className="pl-0 pr-4">
            {userCtx?.currentUser?.isAdminUser ?
              (<ButtonGroup style={{ float: "right" }}>
                <Button
                  className="projectPrimary"
                  disabled={selectedUserIds.length === 0}
                  onClick={() => setMassNotifyPrompt(`Send the following custom message to ${selectedUserIds.length} recipient(s)`)}
                  title="Send a custom message to selected recipients"
                >
                  <FontAwesomeIcon icon="envelope" />
                </Button>
                <Button
                  className="projectSuccess"
                  tag={Link}
                  to="/user/0"
                  title="Add"
                >
                  <FontAwesomeIcon icon='plus' />
                </Button>
                <Button
                  color="secondary"
                  onClick={exportUserXLSX}
                  title="Export Users"
                >
                  <FontAwesomeIcon icon="download" /> Export
                </Button>
              </ButtonGroup>)
              : null
            }
          </Col>
        </Row>
      }
      pager={(<div className="float-right">
          <Pager 
            pageNumber={paginatedList?.pageNumber ? paginatedList.pageNumber : 0}
            totalPages={paginatedList?.totalPages ? paginatedList.totalPages : 0}
            callBack={(newPageNumber) => setPageNumber(newPageNumber)} 
          />
        </div>)}
    >
    <Card>
    <CardBody>
      <Row className="p-0 m-0">
        <Col xs="12">
          {message
            ? (<Alert2 className={message.flavor}>{message.text}</Alert2>)
            : null
          }
          <Table
            striped
            hover
            size="sm"
            responsive={false}
            id="userTable"
          >
            <thead>
              <tr>
                <th width="40px">
                  <input type="checkbox" checked={toggleSelectAll} onChange={(e) => onToggleSelectAll(e.target.checked)} />
                </th>
                      <th width="60px"></th>
                <th>Name</th>
                      <th>Mothers Maiden Name</th>
                      <th>Blood Type</th>
                <th>Contact</th>
                <th>Roles</th>
                <th></th>
                <th width="40%">Info</th>
                <th width="20%"></th>
              </tr>
            </thead>
            <tbody>
              {_.map(paginatedList.list, u => (
                <tr key={`u-${u.id}`}>
                  <td width="40px">
                    <input type="checkbox"
                      checked={selectedUserIds.includes(u.id)}
                      onChange={(e) => {
                        if (e.target.checked) {
                          let newIdList = selectedUserIds.slice();
                          newIdList.push(u.id);
                          setSelectedUserIds(newIdList);
                        } else {
                          setSelectedUserIds(_.reject(selectedUserIds, x => x === u.id));
                        }
                      }} />
                  </td>
                  <td>
                    {u.roleTypeIdList.includes(constants.ROLE_IDS.ADMIN) || u.roleTypeIdList.includes(constants.ROLE_IDS.SYSADMIN)
                      ? <span className="text-success" title="Administrator"><FontAwesomeIcon icon="user" /></span>
                      : null
                    }
                  </td>
                  <td>
                    <Link to={`/user/${u.id}`}>
                      {u.firstName} {u.lastName}
                    </Link>
                    <div style={{fontStyle: "italic", fontWeight: "bold"}}>{u.username}</div>
                  </td>
                  <td>{u.mothersMaidenName}</td>
                  <td>{u.userBloodType}</td>
                  <td>{/*u.email*/}{u.mobilePhoneNumber}</td>
                  <td>{resolveRoleListDescription(u.roles)}</td>
                  <td>
                    {u.mustChangePassword
                      ? (<span
                          className="text-warning clickable-icon" 
                          title='Must change password! - click to remove this requirement'
                          onClick={() => onToggleMustChangePassword(u)}
                          >
                          <FontAwesomeIcon size="2x" icon='exclamation-triangle' />
                        </span>)
                      : null
                    }
                    {u.isLockedOut
                      ? (<span
                          className={"clickable-icon text-danger"} 
                          title="Currently locked out - click to unlock"
                          onClick={() => onUnlockUser(u)}
                          >
                          <FontAwesomeIcon size="2x" icon='unlock-alt' />
                        </span>)
                      : null
                    }
                  </td>
                  <td>
                    <div>Last Logged On: <span style={{fontStyle: "italic"}}>{u.lastLoggedOn ? u.lastLoggedOn : "Never"}</span></div>
                  </td>
                  <td>
                    <ButtonGroup className="float-right">
                      <Button
                        size="sm"
                        title="Impersonate"
                        className="btn-dark"
                        onClick={() => onImpersonate(u)}
                      >
                        <FontAwesomeIcon icon='mask' />
                      </Button>
                      <Button
                        size="sm"
                        className="projectInfo"
                        title="Send Text"
                        onClick={() =>
                          promptText(`${u.firstName} ${u.lastName}`, u.mobilePhoneNumber)}
                      >
                        <FontAwesomeIcon icon='sms' />
                      </Button>
                      {u.mustChangePassword
                        ? null
                        : (<Button
                            className="text-warning" 
                            title='Click to both force change of password and send a link to enable setting to a fresh value.'
                            onClick={() => onToggleMustChangePassword(u)}
                          >
                            <FontAwesomeIcon icon='exclamation-triangle' />
                          </Button>)
                      }
                      {u.deactivatedAt ? (
                        <Button
                          size="sm"
                          className="btn-light"
                          title="Revive"
                          onClick={() => onToggleActive(u)}
                        >
                          <FontAwesomeIcon icon='recycle' />
                        </Button>
                      ) : (
                        <Fragment>
                          <Button
                            size="sm"
                            className="projectWarning"
                            title="Deactivate"
                            onClick={() => onToggleActive(u)}
                          >
                            <FontAwesomeIcon icon='times-circle' />
                          </Button>
                          <Button
                            size="sm"
                            className="projectDanger"
                            title="Delete Permanently"
                            onClick={() => onDelete(u)}
                          >
                            <FontAwesomeIcon icon='times-circle' />
                          </Button>
                        </Fragment>
                      )}
                    </ButtonGroup>
                  </td>
                </tr>))}
            </tbody>
          </Table>
        </Col>
      </Row>
      {massNotifyPrompt ? (
        <Modal isOpen={true} toggle={() => setMassNotifyPrompt("")}>
          <ModalHeader className="primary">
            <FontAwesomeIcon icon="mail-bulk" /> {massNotifyPrompt}
          </ModalHeader>
          <ModalBody>
            <div className="bg-warning p-3 mb-2">
              If a user has provided a mobile phone # it will be used preferentially. Email will be used otherwise.
            </div>
            <Label>Type your message here:</Label>
            <Input
              type="textarea"
              value={textBody}
              onChange={e => setTextBody(e.target.value)}
            />
          </ModalBody>
          <ModalFooter className="float-right">
            <ButtonGroup>
              <Button size="sm" color="primary" onClick={onSendCustomNotification}>
                Send
              </Button>
              <Button
                size="sm"
                color="secondary"
                onClick={() => setMassNotifyPrompt("")}
              >
                Cancel
              </Button>
            </ButtonGroup>
          </ModalFooter>
        </Modal>)
        : null
      }
      {textPhoneNumber ? (
        <Modal isOpen={true} toggle={() => setTextPhoneNumber(null)}>
          <ModalHeader className="primary">
            <FontAwesomeIcon icon="mail-bulk" /> Send custom text to {textFullName}
          </ModalHeader>
          <ModalBody>
            <Label>Type your message here:</Label>
            <Input
              type="textarea"
              value={textBody}
              onChange={e => setTextBody(e.target.value)}
            />
          </ModalBody>
          <ModalFooter className="float-right">
            <ButtonGroup>
              <Button size="sm" color="primary" onClick={sendText}>
                Send
              </Button>
              <Button
                size="sm"
                color="secondary"
                onClick={() => setTextPhoneNumber(null)}
              >
                Cancel
              </Button>
            </ButtonGroup>
          </ModalFooter>
        </Modal>
      ) : null}
    </CardBody>
    </Card>
    </FilterableScreen>
  </DesktopContainer>);
}