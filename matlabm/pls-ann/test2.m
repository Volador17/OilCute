test1;

loadvData;

[y,md,rms,nnd, pScores] = plsannp(vx,bestNet,Weights,Loads,Scores',b,centerCompValue,centerSpecData,Score_length);

pScores = pScores';
vcalY = (vy - centerCompValue)';
% �� pScores��vcalY д���ļ�����FANNʹ��
[row, col] = size(pScores);
fid = fopen('val.data', 'w');
fprintf(fid, '%d %d %d\n', col, row, 1);
for i=1:col
    fprintf(fid,'%f ',pScores(:,i));
    fprintf(fid, '\n');
    fprintf(fid, '%f\n', vcalY(i));
end
fclose(fid);