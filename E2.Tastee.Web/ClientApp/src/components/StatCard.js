import React from "react";
import { Link } from "react-router-dom";
import { Col, Card, Row } from "reactstrap";

export default function StatCard(props) {
  return (
    <Col xs={props.xs || "4"} className="px-1">
      <Card
        className="statCard p-2"
        tag={Link}
        to={{
          pathname: props.link,
          state: { tabName: props.tabName ? props.tabName : null },
          push: true,
        }}
      >
        <Row>
          <Col className="card-title">{props.cardName}</Col>
          <Col className="card-stat text-right pr-4">{props.count}</Col>
        </Row>
      </Card>
    </Col>
  );
}
