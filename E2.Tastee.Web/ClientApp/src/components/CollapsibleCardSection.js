import React, { useState, useEffect } from "react";
import { Card, CardHeader, CardBody, Collapse, CardFooter } from "reactstrap";
import _ from "lodash";

export default function CollapsibleCardSection({
  startOpen,
  titleClass,
  cardName,
  cardClass,
  alwaysOpen,
  cardBody,
  cardFooter
}) {
  let initialOpen = _.isBoolean(startOpen) ? startOpen : true;
  let stayOpen = _.isBoolean(alwaysOpen) ? alwaysOpen : true;
  const [collapseOpen, setCollapseOpen] = useState(initialOpen);

  useEffect(() => {
    let newOpen = _.isBoolean(startOpen) ? startOpen : true;
    setCollapseOpen(newOpen);
  }, [startOpen]);
  return (
    <Card className={cardClass || "collapsibleCardSection"}>
      <CardHeader
        onClick={() => setCollapseOpen(!collapseOpen)}
        disabled={stayOpen}
        className={titleClass || "collapsibleCardSectionTitle"}
      >
        <h5 className="mb-0">{cardName}</h5>
      </CardHeader>
      <Collapse isOpen={collapseOpen}>
        <CardBody className="p-2">{cardBody}</CardBody>
        {cardFooter && <CardFooter>{cardFooter}</CardFooter>}
      </Collapse>
    </Card>
  );
}
