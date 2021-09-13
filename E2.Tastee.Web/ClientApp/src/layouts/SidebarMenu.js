import React, { useState, useContext, useEffect } from "react";
import { NavLink, useLocation } from "react-router-dom";
import classnames from "classnames";
import Sidebar from "react-sidebar";
import _ from "lodash";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FaBars, FaUserCircle, FaWpforms, FaChartLine } from "react-icons/fa";
import { RiListSettingsLine } from "react-icons/ri";
import { RiUserSettingsLine } from "react-icons/ri";
import { TiFlowSwitch } from "react-icons/ti";
// import { BsBuilding } from "react-icons/bs";
// import { GiSteeringWheel, GiTruck, GiReceiveMoney } from "react-icons/gi";
import { FaEdit } from 'react-icons/fa'
import { VscSignOut } from "react-icons/vsc";
import { BsFillHouseDoorFill } from "react-icons/bs";
import {
  Row,
  Col,
  Nav,
  NavItem,
  NavLink as ReactStrapNavLink
} from "reactstrap";
import { Redirect } from "react-router-dom";
import { UserContext, constants } from "../utils";

const homeIcon = (<BsFillHouseDoorFill className="leftMenuButtons align-self-center" />);
const formIcon = <FaWpforms className="leftMenuButtons align-self-center" />;
const userIcon = (<RiUserSettingsLine className="leftMenuButtons align-self-center" />);
const adminIcon = (<RiListSettingsLine className="leftMenuButtons align-self-center" />);
const AdminMenuLinks = [
  { url: "/admin/users", title: "Users", icon: userIcon },
  { url: "/admin/menu", title: "Administration", icon: adminIcon }
];

export default function SidebarMenu() {
  const userCtx = useContext(UserContext);
  const [currentUser] = useState(
    userCtx?.currentUser ? userCtx.currentUser : null
  );
  const [homeLink, setHomeLink] = useState("/dashboard/hauler");
  const [links, setLinks] = useState([]);

  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [openDropdown, setOpenDropdown] = useState(false);
  let location = useLocation();
  const showSwitchButton = currentUser?.roleTypeIdList?.length && currentUser.roleTypeIdList.length > 1;
  // const [redirectToHome, setRedirectToHome] = useState(null);

  useEffect(() => {
    let userLinks = [];
    let homeLink = '';
    if (userCtx?.currentUser?.isAdminUser)
    {
      homeLink = "/dashboard/admin";
      userLinks = userLinks.concat(AdminMenuLinks);
    }
    setHomeLink(homeLink);
    setLinks(userLinks);
  }, [userCtx.currentUser]);


  function toggleSidebar(openOrClose) {
    setSidebarOpen(openOrClose);
  }
  function setUserContext(context) {
    userCtx.setCurrentUserContext(context);
  }
  // only used to close side bar from open sidebar Menu link
  function closeSidebar() {
    if (sidebarOpen) {
      toggleSidebar(false);
    }
  }
  const toggleDropdown = () => setOpenDropdown(!openDropdown);
  const linkItem = (link, index) => {
    if (link.url) {
      return (
        <NavItem
          key={`${sidebarOpen ? "open" : "closed"}Link${index}`}
          title={link.title}
        >
          <ReactStrapNavLink
            tag={NavLink}
            to={link.url}
            active={location.pathname === link.url}
            className="p-0"
          >
            <Row className="mx-0 my-1 p-0 align-items-center justify-content-center menuButtonColor">
              <Col xs={sidebarOpen ? "3" : "12"} className="p-0 m-0">
                {link.icon}
              </Col>
              {sidebarOpen && (
                <Col xs="9" className="text-right">
                  {link.title}
                </Col>
              )}
            </Row>
          </ReactStrapNavLink>
        </NavItem>
      );
    }
  };
  const menuStyling = classnames(
    {
      stickyLeftMenuOpen: sidebarOpen,
      stickyLeftMenuCollapsed: !sidebarOpen,
    },
    "stickyLeftMenu p-0 m-0 py-2"
  );
  // if (redirectToHome !== null) {
  //   return <Redirect to={redirectToHome} />
  // }

  let menu = (
    <div className={menuStyling}>
      <div className="topLeftMenuSection">
        <Row
          className="m-0 mb-2 p-0 align-items-center justify-content-center "
          onClick={() => toggleSidebar(!sidebarOpen)}
        >
          <Col xs={sidebarOpen ? "3" : "12"} className="p-0 m-0">
            <FaBars className="barsButton align-self-center menuButtonColor" />
          </Col>
          {sidebarOpen && (
            <Col xs="9" className="menuButtonColor">
              {userCtx.currentUser.label}
            </Col>
          )}
        </Row>
        <Nav vertical={"true"}>
          {linkItem({ url: homeLink, title: "Dashboard", icon: homeIcon }, -1)}

          {links && links.length > 0 ? (
            <Nav vertical={"true"}>
              {_.map(links, (l, index) => {
                return linkItem(l, index);
              })}
            </Nav>
          ) : null}
        </Nav>
      </div>
      <div className="bottomLeftMenuSection ">
        <Nav vertical={"true"}>
          {showSwitchButton
            ? (
            <NavItem
              key={`${sidebarOpen ? "open" : "closed"}SwitchContext`}
              title="Switch Context"
              onClick={toggleDropdown}
            >
              <Row className="m-0 p-0 align-items-center justify-content-center">
                <Col xs={sidebarOpen ? "3" : "12"} className="p-0 m-0 dropbtn">
                  <TiFlowSwitch className="leftMenuButtons align-self-center menuButtonColor" />
                </Col>
                {sidebarOpen && (
                  <Col xs="9" className="text-right menuButtonColor">
                    Switch Role
                  </Col>
                )}
              </Row>
              {openDropdown && (
                <div>
                  {currentUser.roles.map((r, index) => ( //roleTypeNameList
                    <div key={index} className="dropdown-content" onClick={() => setUserContext(r)}>{r.roleName} - {r.orgName}</div>
                  ))}
                </div>
                )}
            </NavItem>
          ) : null}
          <NavItem
            key={`${sidebarOpen ? "open" : "closed"}LinkProfile`}
            title="Profile"
          >
            <ReactStrapNavLink
              tag={NavLink}
              to={"/profile"}
              className="p-0"
              active={location.pathname === "/profile"}
            >
              <Row
                className="mx-0 my-1 p-0 align-items-center justify-content-center menuButtonColor"
              >
                <Col xs={sidebarOpen ? "3" : "12"} className="p-0 m-0">
                  <FaUserCircle className="leftMenuButtons align-self-center menuButtonColor profileButton" />
                </Col>
                {sidebarOpen && (
                  <Col xs="9" className="text-right">
                    Profile
                  </Col>
                )}
              </Row>
            </ReactStrapNavLink>
          </NavItem>
          <NavItem
            key={`${sidebarOpen ? "open" : "closed"}LinkSignOut`}
            title="Sign Out"
          >
            <ReactStrapNavLink
              onClick={() => userCtx.signOut()}
              className="p-0"
            >
              <Row
                className="mx-0 my-1 p-0 align-items-center justify-content-center menuButtonColor"
              >
                <Col xs={sidebarOpen ? "3" : "12"} className="p-0 m-0">
                  <VscSignOut className="leftMenuButtons align-self-center menuButtonColor signOutButton flipHorizontal" />
                </Col>
                {sidebarOpen && (
                  <Col xs="9" className="text-right">
                    Sign Out
                  </Col>
                )}
              </Row>
            </ReactStrapNavLink>
          </NavItem>
        </Nav>
      </div>
    </div>
  );

  return (
    <Sidebar
      sidebar={sidebarOpen && menu} // opened menu
      open={sidebarOpen}
      styles={{
        sidebar: {
          background: "white",
          zIndex: 2000,
          minWidth: 200,
          // marginTop: 70,
        },
      }}
      pullRight={false}
      onSetOpen={() => toggleSidebar(!sidebarOpen)}
    >
      {!sidebarOpen && menu} {/* collapsed menu */}
    </Sidebar>
  );
}
