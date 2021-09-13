import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router } from 'react-router-dom';
import ReactBreakpoints from "react-breakpoints";
import { ErrorBoundary } from 'react-error-boundary';
import { Button, Row, Col, Container } from "reactstrap";
import { library } from '@fortawesome/fontawesome-svg-core';
import {
  faAngleUp, faAngleDown, faFilter, faExclamationTriangle, faSearch, faClipboardCheck, faSms, faTags, faCalendar, faSync,
  faArrowCircleRight, faArrowCircleLeft, faCheckCircle, faDownload, faHourglass, faHourglassHalf, faBan, faArrowRight, faEnvelopeOpenText,
  faPlusCircle, faEdit, faTimesCircle, faTrash, faRecycle, faUser, faFileUpload, faPrint, faAngleDoubleDown, faFileCsv, faUnlockAlt, faMapMarkedAlt,
  faComments, faImages, faSave, faHome, faEye, faCheck, faPlus, faBackspace, faMinus, faMinusCircle, faGripLines, faTimes, faTrashAlt, faMask, faAward,
  faInfoCircle, faReply, faBars, faUserCircle, faMoneyCheckAlt, faDollarSign, faFileInvoiceDollar, faBuilding, faMailBulk,
  faEnvelope, faMapMarked, faKey, faStar, faStarHalf, faWarehouse, faUsers, faAddressCard, faTruckLoading, faUpload, faUserTimes, faUserPlus,
  faUserSlash, faUserTag, faHandHoldingUsd, faTasks, faExternalLinkAlt, faCheckDouble, faPaperclip, faPaperPlane, faAddressBook,
  faImage, faFilePdf, faBookOpen,
  faLink,
  faEnvelopeOpen,
  faPhoneSquare, faPhone
} from '@fortawesome/free-solid-svg-icons';
import { constants } from "./utils";
import App from './App';
import 'bootstrap/dist/css/bootstrap.css';
import './assets/index.scss';

const rootElement = document.getElementById('root');

library.add(faEye, faAngleUp, faAngleDown, faFilter, faExclamationTriangle, faSearch, faClipboardCheck, faSms, faTags, faCalendar, faSync,
  faCheckCircle, faDownload, faHourglass, faHourglassHalf, faBan, faArrowRight,
  faArrowCircleRight, faArrowCircleLeft, faPlusCircle, faAngleDoubleDown, faFileCsv, faEdit, faTimesCircle,
  faTrash, faRecycle, faInfoCircle, faMapMarkedAlt, faUnlockAlt, faEnvelopeOpenText,
  faUser, faFileUpload, faPrint, faComments, faImages, faSave, faHome, faCheck, faPlus,
  faBackspace, faMinus, faMinusCircle, faGripLines, faTimes, faTrashAlt, faReply, faMask, faAward, faUpload,
  faBars, faUserCircle, faMoneyCheckAlt, faDollarSign, faFileInvoiceDollar, faBuilding, faMailBulk,
  faEnvelope, faMapMarked, faKey, faStar, faStarHalf, faWarehouse, faUsers, faAddressCard, faTruckLoading,
  faUserTimes, faUserPlus, faUserSlash, faUserTag, faHandHoldingUsd, faTasks, faExternalLinkAlt, faCheckDouble, faPaperclip, faPaperPlane, faAddressBook,
  faImage, faLink, faEnvelopeOpen, faPhoneSquare, faPhone, faFilePdf, faBookOpen,
);

const {
  BREAKPOINTS
} = constants;

const goBack = () => {
  window.history.back();
};

const AppFallBackComponent = ({ componentStack, error }) => {
  console.log('componentStack :\n', componentStack, '\n\n error : \n\n', error);
  return (
    <Container>
      <Row className="m-5">
        <Col>
          <h4>Something went wrong on our side.</h4>
        </Col>
      </Row>
      <Row className="m-5">
        <Col>
          <h4>Please use your browser&#39;s back button to return to the previous screen.</h4>
        </Col>
      </Row>
      <Row className="m-5">
        <Col>
          <h4><Button onClick={goBack}>Or click here</Button></h4>
        </Col>
      </Row>
    </Container>
  );
};

ReactDOM.render(
  <ReactBreakpoints breakpoints={BREAKPOINTS}>
    <Router>
      <ErrorBoundary FallbackComponent={AppFallBackComponent}>
        <App />
      </ErrorBoundary>
    </Router>
  </ReactBreakpoints>,
rootElement);

