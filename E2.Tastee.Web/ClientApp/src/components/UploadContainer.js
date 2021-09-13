import React, { useState, useEffect, Fragment } from "react";
import { Col, Button, Row, Table, ButtonGroup } from "reactstrap";
import Alert from "react-s-alert";
import Select from "react-select";
import makeAnimated from "react-select/animated";
import _ from "lodash";
import { api } from "../utils";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { CollapsibleCardSection, Upload } from "../components";
import classnames from "classnames";

export default function UploadContainer(props) {
  const [documentTypeList, setDocumentTypeList] = useState([]);
  const [selectedDocumentType, setSelectedDocumentType] = useState(null);
  const [importProcessing, setImportProcessing] = useState(null);
  const [documentList, setDocumentList] = useState(props.documentList || []);
  const [showUploadModule, setShowUploadModule] = useState(false);

  useEffect(() => {
    if (!props.documentType && documentTypeList.length < 1) {
      if (props.documentTypeList && props.documentTypeList.length > 0) {
        setDocumentTypeList(props.documentTypeList);
      } else {
        api
          .fetch(`public/DocumentTypeList`)
          .then((response) => {
            if (response.data) {
              setDocumentTypeList(
                _.map(response.data, (d) => {
                  d.label = d.name;
                  d.value = d.id;
                  return d;
                })
              );
            }
          })
          .catch((error) => {
            if (props.showError) {
              Alert("There was an error retrieving document types");
            } else {
              console.error(error);
            }
          });
      }
    }
  }, [
    props.documentType,
    props.documentTypeList,
    documentTypeList,
    props.showError,
  ]);

  useEffect(() => {
    if (props.documentList && props.documentList.length > 0) {
      setDocumentList(props.documentList);
    }
  }, [props.documentList]);

  function deleteDocument(index) {
    if (!window.confirm("Are you sure you want to delete this document?"))
      return;
    let list = documentList.slice();
    let id = list[index].id;
    api
      .fetch(`${props.toggleUrl}/${id}`)
      .then((response) => {
        if (response && response.data.success) {
          Alert.success("Document was deleted.");
          if (props.refresh) {
            props.refresh();
          }
        } else {
          Alert.error(response.data.message || "An error occurred.");
        }
      })
      .catch((error) => {
        if (props.showError) {
          Alert("There was an error retrieving document types");
        } else {
          console.error(error);
        }
      });
  }

  function uploadSuccess(acceptedFiles) {
    const formData = new FormData();
    formData.append("file", acceptedFiles[0]);
    props.carrierId && formData.append("CarrierId", props.carrierId);
    props.customerId && formData.append("CustomerId", props.customerId);
    props.loadId && formData.append("LoadId", props.loadId);
    props.userId && formData.append("UserId", props.userId);
    if (!props.documentType && !selectedDocumentType) {
      Alert.error("Document Type is required");
      return;
    }
    if (props.documentType) {
      formData.append("documentTypeId", props.documentType.value);
    } else {
      formData.append("documentTypeId", selectedDocumentType.value);
    }
    api
      .post_form_data(props.uploadUrl, formData)
      .then((response) => {
        if (response.data.success) {
          Alert.success("Document was uploaded");
          setSelectedDocumentType(null);
          if (props.refresh) {
            props.refresh();
          }
          if (props.onUploadOnlySuccess) {
            props.onUploadOnlySuccess(props.documentType);
          }
          setImportProcessing(false);
          setShowUploadModule(false);
        } else {
          Alert.error(
            response.data.message ||
              "An error occurred while processing upload."
          );
          setImportProcessing(false);
          // setUploadResults(null);
        }
      })
      .catch((error) => {
        api.catchHandler(error)
        Alert.error(
          "There was an error uploading your file."
        );
        setImportProcessing(false);
      });
  }


  function uploadFailure(error) {
    Alert.error(
      error ||
        "An error occurred while processing upload."
    );
  }
  let uploadButton = (<Button
    size="sm"
    onClick={() => {
      if (!props.documentType && !selectedDocumentType) {
        Alert.error("Document Type is required");
        return;
      } else {
        setShowUploadModule(!showUploadModule)
      }
    }}
    className={classnames(
      { projectSuccess: !showUploadModule },
      "float-right",
    )}
    disabled={importProcessing || props.disableUpload}
  >
    {showUploadModule ? "Cancel" : props.uploadTitle || "Upload"}
  </Button>)
  if (props.uploadOnly) {
    return (
      <Fragment>
        {uploadButton}
        {showUploadModule &&
          (<Upload
            documentType={props.documentType || selectedDocumentType}
            mimeTypes={props.mimeTypes}
            refresh={props.refresh}
            // MELISSA
            // onUploadSuccess={onUploadSuccess}
            // onUploadFailure={onUploadFailure}
            uploadUrl={props.uploadUrl}
          />
        )}
      </Fragment>
    )
  }
  return (
    <CollapsibleCardSection
      startOpen={true}
      cardClass={"mt-2"}
      cardName={props.cardName}
      cardBody={
        <Fragment>
          <Row className="p-2">
            {props.documentType && !props.documentTypeList ? null : (
              <Col xs="8">
                <Select
                  closeMenuOnSelect={true}
                  isMulti={false}
                  components={makeAnimated()}
                  options={props.documentTypeList || documentTypeList}
                  onChange={(e) => setSelectedDocumentType(e)}
                  value={selectedDocumentType || null}
                  onBlurResetsInput={false}
                  onSelectResetsInput={false}
                  onCloseResetsInput={false}
                  classNamePrefix="react-select"
                />
              </Col>
            )}
            <Col className="float-right">
              {uploadButton}
            </Col>
          </Row>
          {showUploadModule &&
            (<Upload
              documentType={props.documentType || selectedDocumentType}
              mimeTypes={props.mimeTypes}
              refresh={props.refresh}
            />
          )}
          <Row>
            <Col>
              <Table>
                <thead>
                  <tr>
                    <th>File</th>
                    <th>Type</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {documentList && documentList.length > 0 ? (
                    documentList.map((d, index) => (
                      <tr key={index}>
                        <td>
                          <div>{d.name || d.fileName}</div>
                          <div className="dim-text">{d.mimeType}</div>
                        </td>
                        <td>
                          {d.documentTypeOption?.name}
                        </td>
                        <td>
                          <ButtonGroup className="float-right">
                            <Button
                              className="projectPrimary"
                              size="sm"
                              title="View Document"
                              onClick={() =>
                                window.open(
                                  d.signedURL || d.signedUrl,
                                  "_blank"
                                )
                              }
                            >
                              <FontAwesomeIcon icon="external-link-alt" />
                            </Button>
                            <Button
                              size="sm"
                              className="projectDanger"
                              onClick={() => deleteDocument(index)}
                              disabled={!props.toggleUrl}
                            >
                              <FontAwesomeIcon icon="times" />
                            </Button>
                          </ButtonGroup>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td>No documents found</td>
                    </tr>
                  )}
                </tbody>
              </Table>
            </Col>
          </Row>
        </Fragment>
      }
    />
  );
}
