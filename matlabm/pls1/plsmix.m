function [Scores,Loads,Weights,b,Score_length] = plsmix(X,y,k)
    
    [numRowsX,numColsX] = size(X);
    Score_length = zeros(1,k);
    Weights = zeros(numRowsX,k);
    Scores = zeros(numColsX, k);
    b = zeros(k,1);
    Loads = zeros(numRowsX,k);


    for i=1:k
        ww=(X*y/(y'*y));
        Weights(:,i)=(ww/norm(ww))';
        Scores(:,i)=X'*Weights(:,i)/(Weights(:,i)'*Weights(:,i));
        pp=(X*Scores(:,i)/(Scores(:,i)'*Scores(:,i)));
        Loads(:,i)=(pp/norm(pp))';
        b(i) = Scores(:,i)'*y/(Scores(:,i)'*Scores(:,i));
        X=X-Loads(:,i)*Scores(:,i)';
        y=y-b(i)*Scores(:,i);
    end
    
end

