import React, { useContext } from "react";
import { Row, Col, Container } from "reactstrap";
import classnames from "classnames";
import { UserContext} from "../utils";

export default function DesktopContainer(props) {
  const userCtx = useContext(UserContext);
  const currentUser = userCtx && userCtx.currentUser ? userCtx.currentUser : null;
  return (
    <Container fluid className={classnames({
      "desktopContainerPublic": !currentUser,
      "desktopContainer": currentUser,
    }, "")}>
      <Row className="m-0 p-0 fullHeight ">
        <Col className="m-0">
          <Row className="m-0 px-1">
            <Col xs="10" className="my-2 p-0">
              {props.screenName && (<h3 className="primaryDarkText">{props.screenName}</h3>)}
            </Col>
            <Col xs="2" className="m-0 p-0">
            </Col>
          </Row>
          <Row className="m-0 p-0 fullHeight">
            <Col className="m-0 p-0">
              {props.children}
            </Col>
          </Row>
        </Col>
      </Row>
    </Container>
  );
}
