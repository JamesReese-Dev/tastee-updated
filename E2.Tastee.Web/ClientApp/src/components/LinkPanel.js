import React from "react";
import { Link } from "react-router-dom";
import { Col, Row } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
/*
  colSize: *optional* 1-12, default of 4
  routePath: navigate to this path upon click
  title: *optional* visible string
  count: *optional* number to display along with the title
*/
const LinkPanel = props => {
  return (
    <Col sm={props.colSize || "4"} className="px-4 mt-2">
      <Link to={props.routePath} className="no-decoration">
      <Row className="admin-panel" style={{minHeight: "120px"}}>
        <Col sm="12" className="center-vertically">
          <Row className="title pb-2">
            {props.reactIconsKey &&
              (<Col sm="3" className="icon-col mr-0 float-right pr-0 pt-1">
                {props.reactIconsKey}
              </Col>)
            }
            {props.iconKey &&
              (<Col sm="3" className="icon-col mr-0 float-right pr-0 pt-1">
                <FontAwesomeIcon size="3x" icon={props.iconKey} className="linkIcon" />
              </Col>)
            }
            <Col>{props.title}</Col>
          </Row>
        </Col>
      </Row>
      </Link>
    </Col>);
  // return (
  //   <Col
  //     xs="6"
  //     sm={props.colSize || "4"}
  //     className="px-4"
  //   >
  //     <Link to={props.routePath} className="no-decoration">
  //       <Row className="expand-md link-panel mb-3">
  //         {props.iconKey && 
  //           <Col sm="3" className="icon-col mr-0 float-right pr-0 pt-1">
  //             <FontAwesomeIcon
  //               size="2x"
  //               icon={props.iconKey}
  //               className="linkIcon"
  //             />
  //           </Col>
  //         }
  //         <Col>
  //           <div className="title">{props.title}</div>
  //           {props.count ? (<div className="count">{props.count}</div>) : null}
  //         </Col>
  //       </Row>
  //     </Link>
  //   </Col>
  // );
}

export default LinkPanel;
