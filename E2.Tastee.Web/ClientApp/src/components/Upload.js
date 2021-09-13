import React, { useState, Fragment } from "react";
import { Col, Row } from "reactstrap";
import _ from "lodash";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { SneezSpinner } from "../components";
import Dropzone from "react-dropzone";

export default function Upload(props) {
  const [importProcessing, setImportProcessing] = useState(null);

  if (!props.uploadFailure || !props.uploadSuccess) {
    console.log("you need a uploadFailure function and uploadSuccess function props to render this component")
    return null;
  }
  return (
    <Row className="p-2 mt-2">
      <Col>
        <Dropzone
          accept={props.mimeTypes || "image/*,application/pdf,.psd"}
          onDrop={(acceptedFiles) => {
            if (!acceptedFiles || !acceptedFiles[0]) {
              props.uploadFailure("Document format is not accepted")
            }
            console.log('acceptedFiles', acceptedFiles)
            setImportProcessing(true);
            // only allowing 1 at a time right now
            props.uploadSuccess(acceptedFiles[0])
            setImportProcessing(false);
          }}
        >
          {({ getRootProps, getInputProps }) => (
            <section>
              <div
                {...getRootProps()}
                style={{
                  backgroundColor: "#f5f5f5",
                  border: "dashed 1px #000000",
                  padding: "3em",
                  borderRadius: "5px",
                }}
              >
                <input {...getInputProps()} />
                {importProcessing ? (
                  <SneezSpinner
                    on={true}
                    message="Uploading your file now..."
                    className="align-content-center"
                  />
                ) : (
                  <Fragment>
                    <Row className="mb-2" style={{ textAlign: "center" }}>
                      <Col>
                        <FontAwesomeIcon icon="upload" size="4x" />
                      </Col>
                    </Row>
                    <h5>
                      {`Drag file here to upload (or click here to choose file)`}
                    </h5>
                  </Fragment>
                )}
              </div>
            </section>
          )}
        </Dropzone>
      </Col>
    </Row>
  );
}
