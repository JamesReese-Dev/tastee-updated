import React, { useState, useContext } from "react";
import { Row, Col, Card, CardBody } from "reactstrap";
// import _ from "lodash";
import Sidebar from "react-sidebar";
import { MdNotifications, MdNotificationsActive } from "react-icons/md";
import { UserContext } from "../utils";

const MessageCenter = (props) => {
  const [messageSidebarOpen, setMessageSidebarOpen] = useState(false);

  return (
    <Card
      className="messageCenterPopout"
    >
      <CardBody className="pt-1 px-2">
        <Row>
          <Col>MESSAGES HERE</Col>
        </Row>
      </CardBody>
    </Card>
  );
};
export default function MessageHeader(props) {
  const userCtx = useContext(UserContext);
  const [messageSidebarOpen, setMessageSidebarOpen] = useState(false);
  const [messages, setMessages] = useState([]);

  return (
    <Sidebar
      sidebar={<MessageCenter />}
      open={messageSidebarOpen}
      styles={{
        sidebar: {
          background: "white",
          zIndex: 1500,
          minWidth: 800,
          marginTop: 10,
          maxHeight: 900
        },
      }}
      pullRight={true}
      onSetOpen={() => setMessageSidebarOpen(!messageSidebarOpen)}
    >
      <Row className="stickyDupButton p-0">
        <Col className="p-0">
          {messages && messages.length > 0 ? (
            <MdNotificationsActive
              title="Open Messages"
              onClick={() => setMessageSidebarOpen(!messageSidebarOpen)}
              className="successText notificationBellButton"
            />
          ) : (
            <MdNotifications
              title="Open Messages"
              onClick={() => setMessageSidebarOpen(!messageSidebarOpen)}
              className="secondaryText notificationBellButton"
            />
          )}
        </Col>
      </Row>
    </Sidebar>
  );
}
